import { getExperiences, previewBooking, createBooking, getMyBookings, setLogger } from "./api.js";

// Sample only: a fixed tourist. In the real project the backend knows the user from the login.
const DEMO_TOURIST_ID = "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa";

const money = new Intl.NumberFormat("ar-EG", { style: "currency", currency: "EGP" });

const $ = (id) => document.getElementById(id);
let selected = null;

function el(tag, props = {}, children = []) {
  const node = document.createElement(tag);
  Object.assign(node, props);
  for (const child of [].concat(children)) {
    node.append(child);
  }
  return node;
}

function setMessage(text, kind) {
  const m = $("message");
  m.textContent = text;
  m.className = "message" + (kind ? " " + kind : "");
}

// ---------- request log ----------
setLogger(({ method, path, status, ms }) => {
  const ok = typeof status === "number" && status < 400;
  $("log").prepend(
    el("tr", {}, [
      el("td", { textContent: method }),
      el("td", { textContent: path }),
      el("td", { textContent: String(status), className: ok ? "ok-status" : "err-status" }),
      el("td", { textContent: ms + " ms" }),
    ])
  );
});

// ---------- experiences ----------
async function loadExperiences() {
  const box = $("experiences");
  box.replaceChildren();
  try {
    const items = await getExperiences();
    for (const e of items) {
      box.append(
        el("div", { className: "exp" }, [
          el("h3", { textContent: e.title }),
          el("p", { textContent: e.description }),
          el("p", { className: "meta", textContent: `${e.durationMinutes} دقيقة، حد أقصى ${e.maxGroupSize} أفراد، ${e.meetingPoint}` }),
          el("p", { className: "price", textContent: money.format(e.pricePerPerson) + " للفرد" }),
          el("button", { type: "button", textContent: "احجز", onclick: () => selectExperience(e) }),
        ])
      );
    }
  } catch (err) {
    box.append(el("p", { className: "message error", textContent: err.message }));
  }
}

function selectExperience(experience) {
  selected = experience;
  $("selected-title").textContent = experience.title;
  $("guests").max = experience.maxGroupSize;
  $("breakdown").hidden = true;
  $("confirm-btn").hidden = true;
  setMessage("");
  $("booking-section").hidden = false;
  $("booking-section").scrollIntoView({ behavior: "smooth" });
}

function buildPayload() {
  const startValue = $("start").value;
  if (!startValue) throw new Error("اختار موعد البداية.");
  return {
    touristId: DEMO_TOURIST_ID,
    experienceId: selected.id,
    numberOfGuests: Number($("guests").value),
    startAt: new Date(startValue).toISOString(),
  };
}

// ---------- preview + confirm ----------
$("preview-btn").addEventListener("click", async () => {
  setMessage("");
  try {
    const b = await previewBooking(buildPayload());
    const rate = Math.round(b.platformFeeRate * 100);
    const box = $("breakdown");
    box.replaceChildren(
      row("سعر الخدمة", money.format(b.serviceAmount)),
      row(`رسوم المنصة (${rate}%)`, money.format(b.platformFeeAmount)),
      row("الإجمالي", money.format(b.totalAmount), "total")
    );
    box.hidden = false;
    $("confirm-btn").hidden = false;
  } catch (err) {
    $("breakdown").hidden = true;
    $("confirm-btn").hidden = true;
    setMessage(err.message, "error");
  }
});

function row(label, value, extra = "") {
  return el("div", { className: "row " + extra }, [el("span", { textContent: label }), el("span", { textContent: value })]);
}

$("confirm-btn").addEventListener("click", async () => {
  const button = $("confirm-btn");
  button.disabled = true;
  try {
    const booking = await createBooking(buildPayload());
    setMessage(`تم الحجز. الحالة: ${booking.status}`, "ok");
    $("breakdown").hidden = true;
    button.hidden = true;
    await loadBookings();
  } catch (err) {
    setMessage(err.message, "error");
  } finally {
    button.disabled = false;
  }
});

// ---------- my bookings ----------
async function loadBookings() {
  const box = $("bookings");
  box.replaceChildren();
  try {
    const items = await getMyBookings(DEMO_TOURIST_ID);
    if (items.length === 0) {
      box.append(el("p", { className: "empty", textContent: "مفيش حجوزات لسه." }));
      return;
    }
    const table = el("table", {}, [
      el("thead", {}, [el("tr", {}, ["التجربة", "الموعد", "الأفراد", "الإجمالي", "الحالة"].map((h) => el("th", { textContent: h })))]),
      el("tbody", {}, items.map((b) =>
        el("tr", {}, [
          el("td", { textContent: b.experienceTitle }),
          el("td", { textContent: new Date(b.startAt).toLocaleString("ar-EG") }),
          el("td", { textContent: String(b.numberOfGuests) }),
          el("td", { textContent: money.format(b.totalAmount) }),
          el("td", { textContent: b.status }),
        ])
      )),
    ]);
    box.append(table);
  } catch (err) {
    box.append(el("p", { className: "message error", textContent: err.message }));
  }
}

loadExperiences();
loadBookings();
