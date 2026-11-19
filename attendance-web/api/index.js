


const http = require('http');
const crypto = require('crypto');

const SecretSalt = "UniversalGym_Attendance_AntiFraud_Secret_2026";

function getManilaDate() {
    const parts = new Intl.DateTimeFormat('en-US', {
        timeZone: 'Asia/Manila',
        year: 'numeric',
        month: '2-digit',
        day: '2-digit'
    }).formatToParts(new Date());

    const get = type => parts.find(p => p.type === type).value;
    return `${get('year')}-${get('month')}-${get('day')}`;
}

function getManilaTimeString(date = new Date()) {
    return new Intl.DateTimeFormat('en-US', {
        timeZone: 'Asia/Manila',
        hour: 'numeric',
        minute: '2-digit',
        hour12: true
    }).format(date);
}


function generateDailyToken(dateStr, gymLat = 0, gymLng = 0) {
    const latStr = Math.abs(gymLat) > 0.0001
        ? gymLat.toFixed(4)
        : "";

    const lngStr = Math.abs(gymLng) > 0.0001
        ? gymLng.toFixed(4)
        : "";

    const raw = `${SecretSalt}_${dateStr}_${latStr}_${lngStr}`;

    return crypto.createHash("sha256")
        .update(raw, "utf8")
        .digest("hex")
        .substring(0, 16)
        .toUpperCase();
}


function validateDailyToken(dateStr, token) {
    if (!dateStr || !token) {
        return {
            valid: false,
            error: 'Invalid QR code. Please scan the QR code at the counter.'
        };
    }

    const today = getManilaDate();

    if (dateStr < today) {
        return {
            valid: false,
            error: "Expired QR code. Please scan today's active QR code."
        };
    }

    if (dateStr !== today ||
        token.toUpperCase() !== generateDailyToken(today)) {
        return {
            valid: false,
            error: 'Invalid QR code.'
        };
    }

    return { valid: true };
}

// Supabase database connection
async function supabaseRequest(method, resource, body) {
    const base = (process.env.SUPABASE_URL || '').replace(/\/$/, '');
    const key = process.env.SUPABASE_SECRET_KEY;

    if (!base || !key) {
        throw new Error('Supabase environment variables are missing.');
    }

    const response = await fetch(`${base}/rest/v1/${resource}`, {
        method,
        headers: {
            'apikey': key,
            'Authorization': `Bearer ${key}`,
            'Content-Type': 'application/json',
            'Prefer': method === 'POST'
                ? 'return=representation'
                : 'return=minimal'
        },

        ...(body === undefined
            ? {}
            : { body: JSON.stringify(body) })
    });

    const text = await response.text();
    let data;

    try {
        data = text ? JSON.parse(text) : null;
    } catch {
        data = text;
    }

    if (!response.ok) {
        console.error('Supabase error:', response.status, data);

        const error = new Error('Database request failed.');
        error.dbCode = data?.code;
        throw error;
    }

    return data;
}

function sendJson(res, status, data) {
    res.writeHead(status, {
        'Content-Type': 'application/json; charset=utf-8',
        'Cache-Control': 'no-store'
    });

    res.end(JSON.stringify(data));
}

async function readBody(req) {
    let raw = '';

    for await (const chunk of req) {
        raw += chunk.toString();

        if (raw.length > 16384) {
            const error = new Error('Request too large.');
            error.status = 413;
            throw error;
        }
    }

    return raw ? JSON.parse(raw) : {};
}

function isSyncAuthorized(req) {
    const configuredKey = process.env.SYNC_API_KEY;
    const suppliedKey = req.headers.authorization || '';

    if (!configuredKey || !suppliedKey.startsWith('Bearer ')) {
        return false;
    }

    const received = Buffer.from(suppliedKey.slice(7));
    const expected = Buffer.from(configuredKey);

    return received.length === expected.length &&
        crypto.timingSafeEqual(received, expected);
}

function toSyncRecord(row) {
    return {
        id: row.id,
        memberId: row.member_id,
        fullName: row.member_name,
        attendanceDate: row.attendance_date,
        checkInTime: getManilaTimeString(new Date(row.check_in_time)),
        recordedAt: row.check_in_time,
        recordedBy: row.recorded_by || 'QR-Mobile',
        synced: row.synced === true
    };
}

// Attendance website
function renderHtmlPage() {
    return `<!DOCTYPE html>
<html lang="en">
<head>
<meta charset="UTF-8">
<meta name="viewport" content="width=device-width, initial-scale=1.0">
<title>Universal Gym - Member Attendance</title>

<style>
* {
  box-sizing: border-box;
  margin: 0;
  padding: 0;
  font-family: Arial, sans-serif;
}

body {
  background: #0f172a;
  color: white;
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
  padding: 16px;
}

.card {
  background: #1e293b;
  width: 100%;
  max-width: 440px;
  border-radius: 20px;
  overflow: hidden;
  border: 1px solid #334155;
}

.header {
  background: #0f172a;
  text-align: center;
  padding: 28px 20px;
  border-bottom: 4px solid #ff8423;
}

.header h1 {
  font-size: 26px;
  font-weight: 900;
}

.header p {
  color: #94a3b8;
  font-size: 13px;
  margin-top: 6px;
}

.body {
  padding: 24px 20px;
}

.form-group {
  margin-bottom: 16px;
}

label {
  display: block;
  font-size: 12px;
  font-weight: bold;
  color: #94a3b8;
  margin-bottom: 6px;
}

input {
  width: 100%;
  padding: 13px;
  background: #0f172a;
  color: white;
  border: 1px solid #334155;
  border-radius: 10px;
  font-size: 16px;
}

input:focus {
  outline: none;
  border-color: #ff8423;
}

button {
  width: 100%;
  padding: 15px;
  background: #ff8423;
  border: none;
  border-radius: 10px;
  color: white;
  font-weight: bold;
  font-size: 16px;
  cursor: pointer;
}

button:disabled {
  background: #475569;
}

.result {
  display: none;
  padding: 16px;
  border-radius: 10px;
  margin-top: 18px;
  line-height: 1.6;
  font-size: 14px;
  white-space: pre-line;
}

.success {
  display: block;
  background: #064e3b;
  color: #a7f3d0;
}

.error {
  display: block;
  background: #7f1d1d;
  color: #fecaca;
}

.footer {
  text-align: center;
  color: #64748b;
  font-size: 11px;
  margin-top: 24px;
}
</style>
</head>

<body>
<div class="card">

  <div class="header">
    <h1>UNIVERSAL GYM</h1>
    <p>Member Attendance Check-In</p>
  </div>

  <div class="body">
    <form id="attendanceForm">

      <div class="form-group">
        <label for="memberId">MEMBER ID</label>
        <input id="memberId" type="text"
          placeholder="e.g. GM100001" required>
      </div>

      <div class="form-group">
        <label for="firstName">FIRST NAME</label>
        <input id="firstName" type="text"
          placeholder="e.g. Juan" required>
      </div>

      <div class="form-group">
        <label for="lastName">LAST NAME</label>
        <input id="lastName" type="text"
          placeholder="e.g. Dela Cruz" required>
      </div>

      <button id="btnSubmit" type="submit">
        Submit Attendance
      </button>

    </form>

    <div id="resultBox" class="result"></div>
    <div class="footer">Universal Gym Attendance</div>
  </div>
</div>

<script>
const params = new URLSearchParams(window.location.search);
const qrDate = params.get('date') || '';
const qrToken = params.get('token') || '';

const form = document.getElementById('attendanceForm');
const resultBox = document.getElementById('resultBox');
const btn = document.getElementById('btnSubmit');

form.addEventListener('submit', async function(event) {
  event.preventDefault();

  btn.disabled = true;
  btn.textContent = 'Submitting...';
  resultBox.className = 'result';
  resultBox.style.display = 'none';

  const payload = {
    memberId: document.getElementById('memberId').value.trim(),
    firstName: document.getElementById('firstName').value.trim(),
    lastName: document.getElementById('lastName').value.trim(),
    qrDate,
    qrToken
  };

  try {
    const response = await fetch('/api/attendance', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(payload)
    });

    const data = await response.json();

    resultBox.className = data.success
      ? 'result success'
      : 'result error';

    if (data.success) {
      resultBox.textContent =
        'Check-in Recorded!\\n' +
        data.fullName + ' (' + data.memberId + ')\\n' +
        'Time: ' + data.checkInTime;

      form.reset();
    } else {
      resultBox.textContent =
        'Check-in Failed\\n' +
        (data.message || 'Please try again.');
    }

  } catch (error) {
    resultBox.className = 'result error';
    resultBox.textContent =
      'Connection Error\\nCould not submit attendance.';
  } finally {
    resultBox.style.display = 'block';
    btn.disabled = false;
    btn.textContent = 'Submit Attendance';
  }
});
</script>
</body>
</html>`;
}

// API request handler
async function handler(req, res) {
    const pathname = new URL(
        req.url,
        'http://localhost'
    ).pathname;

    // Health check
    if (req.method === 'GET' && pathname === '/api/health') {
        return sendJson(res, 200, {
            status: 'ok',
            serverTime: new Date().toISOString(),
            manilaDate: getManilaDate()
        });
    }

    // Attendance page
    if (req.method === 'GET' &&
        ['/', '/attendance', '/index.html'].includes(pathname)) {
        res.setHeader('Content-Type', 'text/html; charset=utf-8');
        return res.end(renderHtmlPage());
    }

    // Attendance submission
    if (req.method === 'POST' && pathname === '/api/attendance') {
        try {
            const payload = await readBody(req);

            const memberId = String(payload.memberId || '')
                .trim().toUpperCase();

            const firstName = String(payload.firstName || '').trim();
            const lastName = String(payload.lastName || '').trim();
            const qrDate = String(payload.qrDate || '').trim();
            const qrToken = String(payload.qrToken || '').trim();

            if (!memberId || !firstName || !lastName) {
                return sendJson(res, 400, {
                    success: false,
                    message: 'All fields are required.'
                });
            }

            if (memberId.length > 50 ||
                firstName.length > 100 ||
                lastName.length > 100) {
                return sendJson(res, 400, {
                    success: false,
                    message: 'One or more fields are too long.'
                });
            }

            const validation = validateDailyToken(qrDate, qrToken);

            if (!validation.valid) {
                return sendJson(res, 400, {
                    success: false,
                    message: validation.error
                });
            }

            const today = getManilaDate();
            const fullName = `${firstName} ${lastName}`;

            const existing = await supabaseRequest(
                'GET',
                `attendance_records?select=id&member_id=eq.${encodeURIComponent(memberId)}&attendance_date=eq.${today}&is_deleted=eq.false&limit=1`
            );

            if (Array.isArray(existing) && existing.length > 0) {
                return sendJson(res, 200, {
                    success: false,
                    message: `Member ${memberId} has already checked in today!`,
                    memberId,
                    fullName
                });
            }

            const record = {
                id: `ATT-${crypto.randomUUID()}`,
                member_id: memberId,
                member_name: fullName,
                attendance_date: today,
                check_in_time: new Date().toISOString(),
                recorded_by: 'QR-Mobile',
                is_deleted: false,
                synced: false
            };

            try {
                await supabaseRequest(
                    'POST',
                    'attendance_records',
                    record
                );
            } catch (error) {
                if (error.dbCode === '23505') {
                    return sendJson(res, 200, {
                        success: false,
                        message: `Member ${memberId} has already checked in today!`,
                        memberId,
                        fullName
                    });
                }

                throw error;
            }

            return sendJson(res, 200, {
                success: true,
                message: 'Attendance recorded successfully!',
                memberId,
                fullName,
                checkInTime: getManilaTimeString()
            });

        } catch (error) {
            console.error('Attendance error:', error);

            return sendJson(res, error.status || 500, {
                success: false,
                message: 'Unable to save attendance. Please try again.'
            });
        }
    }

    // Protect desktop synchronization endpoints
    if (pathname === '/api/sync' ||
        pathname === '/api/sync/ack') {
        if (!isSyncAuthorized(req)) {
            return sendJson(res, 401, {
                success: false,
                message: 'Unauthorized synchronization request.'
            });
        }
    }

    // Get unsynced attendance records
    if (req.method === 'GET' && pathname === '/api/sync') {
        try {
            const rows = await supabaseRequest(
                'GET',
                'attendance_records?select=*&synced=eq.false&is_deleted=eq.false&order=check_in_time.asc&limit=1000'
            );

            const records = rows.map(toSyncRecord);

            return sendJson(res, 200, {
                success: true,
                count: records.length,
                records
            });

        } catch (error) {
            console.error('Sync error:', error);

            return sendJson(res, 500, {
                success: false,
                message: 'Unable to retrieve attendance.'
            });
        }
    }

    // Acknowledge synchronized records
    if (req.method === 'POST' && pathname === '/api/sync/ack') {
        try {
            const body = await readBody(req);

            const ids = Array.isArray(body.syncedIds)
                ? body.syncedIds
                : [];

            if (ids.length > 1000 ||
                ids.some(id =>
                    typeof id !== 'string' ||
                    !/^ATT-[a-zA-Z0-9-]+$/.test(id)
                )) {
                return sendJson(res, 400, {
                    success: false,
                    message: 'Invalid synchronization IDs.'
                });
            }

            if (ids.length > 0) {
                const encodedIds = ids.map(id => `"${id}"`).join(',');

                await supabaseRequest(
                    'PATCH',
                    `attendance_records?id=in.(${encodeURIComponent(encodedIds)})`,
                    { synced: true }
                );
            }

            return sendJson(res, 200, {
                success: true,
                acknowledged: ids.length
            });

        } catch (error) {
            console.error('Acknowledgement error:', error);

            return sendJson(res, 500, {
                success: false,
                message: 'Unable to acknowledge synchronization.'
            });
        }
    }

    return sendJson(res, 404, {
        success: false,
        message: 'Endpoint not found.'
    });
}

// Local server
if (require.main === module) {
    const PORT = process.env.PORT || 3000;

    http.createServer(handler).listen(PORT, () => {
        console.log(`Attendance API running on port ${PORT}`);
    });
}

// Vercel export
module.exports = handler;


