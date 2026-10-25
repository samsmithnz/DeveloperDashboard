document.getElementById("sync-demo")?.addEventListener("click", () => {
    const feedback = document.getElementById("sync-feedback");
    feedback.hidden = false;
    feedback.textContent = "Prototype only: no sync was started. In the live app, an admin can request collection here and see progress, throttling, or an already-running sync.";
});
