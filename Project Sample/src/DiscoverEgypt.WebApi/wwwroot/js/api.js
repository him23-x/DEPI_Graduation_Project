// The ONLY file that talks to the backend.
// If an endpoint changes, you change it here and nowhere else.

// Same address as the page (the page is served by the Web API itself, so no CORS is needed).
// If the frontend runs on another address, set this to the API address, for example "http://localhost:5080",
// and enable CORS in Program.cs.
const API_BASE = "";

let logger = () => {};
export function setLogger(fn) {
  logger = fn;
}

async function request(method, path, body) {
  const started = performance.now();
  const options = { method };
  if (body) {
    options.headers = { "Content-Type": "application/json" };
    options.body = JSON.stringify(body);
  }

  let response;
  try {
    response = await fetch(API_BASE + path, options);
  } catch (networkError) {
    logger({ method, path, status: "network error", ms: Math.round(performance.now() - started) });
    throw new Error("مقدرناش نوصل للـ API. تأكد إن المشروع شغّال.");
  }

  const data = await response.json().catch(() => null);
  logger({ method, path, status: response.status, ms: Math.round(performance.now() - started) });

  if (!response.ok) {
    throw new Error(data?.error ?? `الطلب فشل (${response.status})`);
  }
  return data;
}

export const getExperiences = () => request("GET", "/api/experiences");
export const previewBooking = (payload) => request("POST", "/api/bookings/preview", payload);
export const createBooking = (payload) => request("POST", "/api/bookings", payload);
export const getMyBookings = (touristId) => request("GET", `/api/bookings?touristId=${encodeURIComponent(touristId)}`);
