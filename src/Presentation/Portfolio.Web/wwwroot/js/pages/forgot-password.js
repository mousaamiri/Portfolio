/* ==========================================================================
   forgot-password.js — Forgot password page behavior
   ========================================================================== */

(function () {
  "use strict";

  var form = document.getElementById("forgotForm");
  var emailInput = document.getElementById("forgotEmail");
  var submitBtn = document.getElementById("forgotSubmitBtn");
  var card = document.getElementById("forgotCard");
  var step1 = document.getElementById("forgotStep1");
  var step2 = document.getElementById("forgotStep2");
  var sentEmailBadge = document.getElementById("sentEmailBadge");
  var resendBtn = document.getElementById("resendBtn");
  var logoImg = document.getElementById("forgotLogo");

  // --- Logo switching based on theme + language ---
  function updateLogo() {
    if (!logoImg) return;
    var theme = document.documentElement.getAttribute("data-theme") || "dark";
    var lang = document.documentElement.getAttribute("lang") || "en";
    var langKey = lang === "fa" ? "fa" : "en";
    var themeKey = theme === "light" ? "light" : "dark";
    logoImg.src = "/images/logo-" + langKey + "-" + themeKey + ".svg";
  }

  updateLogo();

  var observer = new MutationObserver(function (mutations) {
    mutations.forEach(function (m) {
      if (m.attributeName === "data-theme" || m.attributeName === "lang") {
        updateLogo();
      }
    });
  });
  observer.observe(document.documentElement, { attributes: true });

  if (!form) return;

  function validateEmail(email) {
    if (!email.trim()) return "Email is required";
    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) return "Please enter a valid email";
    return "";
  }

  function showError(message) {
    var group = document.getElementById("forgotEmailGroup");
    var error = document.getElementById("forgotEmailError");
    if (group) group.classList.toggle("has-error", !!message);
    if (error) error.textContent = message || "";
  }

  if (emailInput) {
    emailInput.addEventListener("input", function () {
      showError("");
    });
  }

  form.addEventListener("submit", function (e) {
    e.preventDefault();

    var email = emailInput ? emailInput.value : "";
    var err = validateEmail(email);

    if (err) {
      showError(err);
      if (card) {
        card.classList.remove("shake");
        void card.offsetWidth;
        card.classList.add("shake");
      }
      return;
    }

    submitBtn.disabled = true;
    submitBtn.classList.add("loading");

    setTimeout(function () {
      submitBtn.disabled = false;
      submitBtn.classList.remove("loading");

      if (sentEmailBadge) {
        sentEmailBadge.textContent = email;
      }

      if (step1) step1.classList.add("forgot-step--hidden");
      if (step2) step2.classList.remove("forgot-step--hidden");

      card.classList.remove("shake");
      card.style.animation = "none";
      void card.offsetWidth;
      card.style.animation = "";
      card.style.opacity = "0";
      card.style.transform = "translateY(20px)";
      requestAnimationFrame(function () {
        card.style.transition = "opacity 0.4s ease, transform 0.4s ease";
        card.style.opacity = "1";
        card.style.transform = "translateY(0)";
      });
    }, 1500);
  });

  if (resendBtn) {
    resendBtn.addEventListener("click", function () {
      resendBtn.disabled = true;
      resendBtn.textContent = "Sent!";

      setTimeout(function () {
        resendBtn.disabled = false;
        resendBtn.textContent = "Resend";
      }, 3000);
    });
  }
})();
