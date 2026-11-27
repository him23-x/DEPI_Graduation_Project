document.addEventListener("click", async (event) => {
    const button = event.target.closest("[data-copy-booking-share]");
    if (!button) return;

    const input = document.getElementById(button.dataset.copyBookingShare);
    const feedback = document.getElementById(button.dataset.copyFeedback);
    if (!input) return;

    try {
        await navigator.clipboard.writeText(input.value);
        if (feedback) feedback.textContent = "Link copied.";
    } catch {
        input.focus();
        input.select();
        const copied = document.execCommand("copy");
        if (feedback) feedback.textContent = copied ? "Link copied." : "Select and copy the link.";
    }
});
