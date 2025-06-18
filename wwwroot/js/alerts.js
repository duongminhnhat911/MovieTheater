document.addEventListener("DOMContentLoaded", function () {
    function fadeOutAndRemove(element) {
        setTimeout(() => {
            element.style.transition = "opacity 0.5s ease";
            element.style.opacity = "0";
            setTimeout(() => {
                element.remove();
                window.location.replace(window.location.pathname);
            }, 500);
        }, 3000);
    }
    const success = document.getElementById("alertSuccess");
    const danger = document.getElementById("alertDanger");

    if (success) fadeOutAndRemove(success);
    if (danger) fadeOutAndRemove(danger);
});
