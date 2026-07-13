/* ==========================================================================
   contact.js — Contact page interactivity
   --------------------------------------------------------------------------
   Live clock, interest pills, FAQ accordion, form submit, copy to clipboard.
   ========================================================================== */

(function () {
  "use strict";

  /* ------------------------------------------------------------------
     1. Live Clock — updates every second with IST (GMT+5:30)
     ------------------------------------------------------------------ */

  var timeEl = document.getElementById("localTime");

  function updateClock() {
    var now = new Date();
    // Format in IST (Asia/Kolkata)
    var options = {
      timeZone: "Asia/Kolkata",
      weekday: "short",
      year: "numeric",
      month: "short",
      day: "numeric",
      hour: "2-digit",
      minute: "2-digit",
      second: "2-digit",
      hour12: true,
    };
    var formatted = now.toLocaleString("en-US", options);
    // Add timezone suffix
    if (timeEl) {
      timeEl.textContent = formatted + " (GMT+5:30)";
    }
  }

  if (timeEl) {
    updateClock();
    setInterval(updateClock, 1000);
  }

  /* ------------------------------------------------------------------
     2. Interest Pill Selection
     ------------------------------------------------------------------ */

  var pills = document.querySelectorAll(".contact-pill");

  pills.forEach(function (pill) {
    pill.addEventListener("click", function () {
      pills.forEach(function (p) {
        p.classList.remove("active");
      });
      pill.classList.add("active");
    });
  });

  /* ------------------------------------------------------------------
     3. FAQ Accordion
     ------------------------------------------------------------------ */

  var faqItems = document.querySelectorAll(".contact-faq-item");

  faqItems.forEach(function (item) {
    var toggle = item.querySelector(".contact-faq-toggle");
    if (!toggle) return;

    toggle.addEventListener("click", function () {
      var isOpen = item.classList.contains("open");

      // Close all other items
      faqItems.forEach(function (other) {
        if (other !== item) {
          other.classList.remove("open");
          var otherToggle = other.querySelector(".contact-faq-toggle");
          if (otherToggle) otherToggle.setAttribute("aria-expanded", "false");
        }
      });

      // Toggle current
      item.classList.toggle("open");
      toggle.setAttribute("aria-expanded", isOpen ? "false" : "true");
    });
  });

  /* ------------------------------------------------------------------
     4. Copy to Clipboard
     ------------------------------------------------------------------ */

  var copyBtns = document.querySelectorAll(".contact-copy-btn");

  copyBtns.forEach(function (btn) {
    btn.addEventListener("click", function () {
      var textToCopy = btn.getAttribute("data-copy");
      if (!textToCopy) return;

      navigator.clipboard.writeText(textToCopy).then(function () {
        btn.classList.add("copied");

        // Replace icon temporarily with a check
        var originalHTML = btn.innerHTML;
        btn.innerHTML =
          '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" width="16" height="16">' +
          '<polyline points="20 6 9 17 4 12"/>' +
          "</svg>";

        setTimeout(function () {
          btn.classList.remove("copied");
          btn.innerHTML = originalHTML;
        }, 2000);
      }).catch(function () {
        // Fallback for older browsers
        var ta = document.createElement("textarea");
        ta.value = textToCopy;
        ta.style.position = "fixed";
        ta.style.opacity = "0";
        document.body.appendChild(ta);
        ta.select();
        try {
          document.execCommand("copy");
          btn.classList.add("copied");
          var originalHTML = btn.innerHTML;
          btn.innerHTML =
            '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" width="16" height="16">' +
            '<polyline points="20 6 9 17 4 12"/>' +
            "</svg>";
          setTimeout(function () {
            btn.classList.remove("copied");
            btn.innerHTML = originalHTML;
          }, 2000);
        } catch (e) {
          /* silent */
        }
        document.body.removeChild(ta);
      });
    });
  });

  /* ------------------------------------------------------------------
     5. Form Submission
     ------------------------------------------------------------------ */

  var form = document.getElementById("contactForm");
  var feedback = document.getElementById("formFeedback");

  if (form) {
    form.addEventListener("submit", function (e) {
      e.preventDefault();

      // Basic validation
      var name = form.querySelector("#contactName");
      var email = form.querySelector("#contactEmail");
      var phone = form.querySelector("#contactPhone");
      var subject = form.querySelector("#contactSubject");
      var message = form.querySelector("#contactMessage");

      if (!name.value.trim() || !email.value.trim() || !message.value.trim()) {
        if (feedback) {
          if (!name.value.trim()) {
            feedback.textContent = window.i18n ? window.i18n.t("contact.error_name", "Please enter your name.") : "Please enter your name.";
          } else if (!email.value.trim()) {
            feedback.textContent = window.i18n ? window.i18n.t("contact.error_email", "Please enter a valid email address.") : "Please enter a valid email address.";
          } else {
            feedback.textContent = window.i18n ? window.i18n.t("contact.error_message", "Please enter your message.") : "Please enter your message.";
          }
          feedback.className = "contact-form-feedback error";
        }
        return;
      }

      // Email format check
      var emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
      if (!emailPattern.test(email.value.trim())) {
        if (feedback) {
          feedback.textContent = window.i18n ? window.i18n.t("contact.error_email", "Please enter a valid email address.") : "Please enter a valid email address.";
          feedback.className = "contact-form-feedback error";
        }
        return;
      }

      // Get selected interest
      var activePill = document.querySelector(".contact-pill.active");
      var interest = activePill ? activePill.getAttribute("data-interest") : "general";

      // Real submission — POST to the server, which forwards to Portfolio.API
      // and persists the message (admin inbox). Same UI as before on success.
      var submitBtn = form.querySelector(".contact-submit-btn");
      var originalText = submitBtn.innerHTML;
      submitBtn.innerHTML = "Sending...";
      submitBtn.disabled = true;

      function showError(msg) {
        if (feedback) {
          feedback.textContent = msg;
          feedback.className = "contact-form-feedback error";
        }
        submitBtn.innerHTML = originalText;
        submitBtn.disabled = false;
      }

      fetch("/Contact/Submit", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          name: name.value.trim(),
          email: email.value.trim(),
          phone: phone ? phone.value.trim() : "",
          subject: subject ? subject.value.trim() : "",
          message: message.value.trim(),
          interest: interest
        })
      })
        .then(function (res) {
          if (res.status === 429) {
            showError(window.i18n ? window.i18n.t("contact.error_ratelimit", "Too many attempts. Please wait a few minutes and try again.") : "Too many attempts. Please wait a few minutes and try again.");
            return null;
          }
          return res.ok ? res.json() : { success: false };
        })
        .then(function (data) {
          if (data === null) return; // rate-limited; message already shown
          if (!data || !data.success) {
            showError(window.i18n ? window.i18n.t("contact.error_generic", "Something went wrong. Please try again.") : "Something went wrong. Please try again.");
            return;
          }
          if (feedback) {
            feedback.textContent = window.i18n ? window.i18n.t("contact.success", "Message sent successfully! I'll get back to you soon.") : "Message sent successfully! I'll get back to you soon.";
            feedback.className = "contact-form-feedback success";
          }
          submitBtn.innerHTML = originalText;
          submitBtn.disabled = false;
          form.reset();

          // Re-select default pill
          pills.forEach(function (p) { p.classList.remove("active"); });
          if (pills.length > 0) pills[0].classList.add("active");

          // Clear feedback after a few seconds
          setTimeout(function () {
            if (feedback) {
              feedback.textContent = "";
              feedback.className = "contact-form-feedback";
            }
          }, 5000);
        })
        .catch(function () {
          showError(window.i18n ? window.i18n.t("contact.error_generic", "Something went wrong. Please try again.") : "Something went wrong. Please try again.");
        });
    });
  }
})();

/* ==========================================================================
   Text reveals (uses revealText from text-reveal.js)
   ========================================================================== */
(function () {
  "use strict";

  if (typeof window.revealText !== "function") return;

  var REVEALS = [
    [".contact-heading", { type: "chars", stagger: 0.025, blurAmount: 14, duration: 0.6 }],
    [".contact-info-title", { type: "words", stagger: 0.04, blurAmount: 10, duration: 0.5 }],
    [".contact-info-desc", { type: "words", stagger: 0.03, blurAmount: 8, duration: 0.5 }],
    [".contact-detail-label", { type: "chars", stagger: 0.02, blurAmount: 8, duration: 0.4 }],
    [".contact-map-location", { type: "chars", stagger: 0.02, blurAmount: 10, duration: 0.5 }],
    [".closing-cta-heading", { type: "chars", stagger: 0.02, blurAmount: 14, duration: 0.6 }],
    [".closing-cta-card-title", { type: "chars", stagger: 0.02, blurAmount: 10, duration: 0.5 }],
    [".closing-cta-card-desc", { type: "words", stagger: 0.03, blurAmount: 8, duration: 0.45 }],
  ];

  REVEALS.forEach(function (entry) {
    window.revealText(entry[0], Object.assign({ scroll: true }, entry[1]));
  });
})();
