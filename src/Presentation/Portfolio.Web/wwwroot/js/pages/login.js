/* ==========================================================================
   login.js — Login page behavior
   ========================================================================== */

(function () {
  "use strict";

  var form = document.getElementById("loginForm");
  var emailInput = document.getElementById("loginEmail");
  var passwordInput = document.getElementById("loginPassword");
  var toggleBtn = document.getElementById("togglePassword");
  var submitBtn = document.getElementById("loginSubmitBtn");
  var feedback = document.getElementById("loginFeedback");
  var card = document.querySelector(".login-card");
  var logoImg = document.getElementById("loginLogo");

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

  // --- Password toggle ---
  if (toggleBtn && passwordInput) {
    toggleBtn.addEventListener("click", function () {
      var isPassword = passwordInput.type === "password";
      passwordInput.type = isPassword ? "text" : "password";
      toggleBtn.classList.toggle("active", isPassword);
      toggleBtn.setAttribute("aria-label", isPassword ? "Hide password" : "Show password");
    });
  }

  // --- Validation ---
  function validateEmail(email) {
    if (!email.trim()) return "Email is required";
    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) return "Please enter a valid email";
    return "";
  }

  function validatePassword(password) {
    if (!password) return "Password is required";
    if (password.length < 6) return "Password must be at least 6 characters";
    return "";
  }

  function showFieldError(groupId, errorId, message) {
    var group = document.getElementById(groupId);
    var error = document.getElementById(errorId);
    if (group) group.classList.toggle("has-error", !!message);
    if (error) error.textContent = message || "";
  }

  function clearErrors() {
    showFieldError("emailGroup", "emailError", "");
    showFieldError("passwordGroup", "passwordError", "");
    if (feedback) {
      feedback.textContent = "";
      feedback.className = "login-form-feedback";
    }
  }

  if (emailInput) {
    emailInput.addEventListener("input", function () {
      showFieldError("emailGroup", "emailError", "");
    });
  }

  if (passwordInput) {
    passwordInput.addEventListener("input", function () {
      showFieldError("passwordGroup", "passwordError", "");
    });
  }

  // --- Form submit ---
  form.addEventListener("submit", function (e) {
    e.preventDefault();
    clearErrors();

    var email = emailInput ? emailInput.value : "";
    var password = passwordInput ? passwordInput.value : "";

    var emailErr = validateEmail(email);
    var passwordErr = validatePassword(password);

    if (emailErr || passwordErr) {
      showFieldError("emailGroup", "emailError", emailErr);
      showFieldError("passwordGroup", "passwordError", passwordErr);

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

      if (feedback) {
        feedback.textContent = "Login successful! Redirecting...";
        feedback.className = "login-form-feedback success";
      }
    }, 1800);
  });
})();
