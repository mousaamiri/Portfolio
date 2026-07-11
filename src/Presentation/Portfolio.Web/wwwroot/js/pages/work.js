/* ==========================================================================
   work.js — Project showcase interactive logic
   --------------------------------------------------------------------------
   Renders the project list, handles selection, and updates the detail panel.
   Optional GSAP fade transition on project switch.
   ========================================================================== */

(function () {
  "use strict";

  /* ---- Get active projects ---- */
  // Projects are the SINGLE source of truth from the server (Portfolio.API via
  // PortfolioApiClient). The Razor view serializes the already-localized list
  // into a JSON data-island (<script type="application/json" id="work-data">)
  // that lives INSIDE #page-content, so it is guaranteed present in the DOM on
  // both a full page load and an SPA content swap — before this script reads it.
  // No hardcoded/mock/placeholder data lives here anymore.
  var _cachedProjects = null;

  function getActiveProjects() {
    if (_cachedProjects) return _cachedProjects;

    var dataEl = document.getElementById("work-data");
    if (dataEl) {
      try {
        _cachedProjects = JSON.parse(dataEl.textContent || "[]");
      } catch (e) {
        _cachedProjects = [];
      }
    } else {
      // Backwards-compat: legacy global injection. Empty array if absent —
      // we never fabricate data on the client.
      _cachedProjects = window.__workProjects || [];
    }
    return _cachedProjects;
  }

  /* ---- DOM references ---- */
  var listEl = document.querySelector(".work-list");
  var detailInner = document.querySelector(".work-detail-inner");
  var detailNumber = document.getElementById("detail-number");
  var detailTitle = document.getElementById("detail-title");
  var detailSubtitle = document.getElementById("detail-subtitle");
  var detailDesc = document.getElementById("detail-desc");
  var detailTechs = document.getElementById("detail-techs");
  var detailGithub = document.getElementById("detail-github");
  var detailPreviewImg = document.getElementById("detail-preview-img");
  var detailPreviewIcon = document.getElementById("detail-preview-icon");

  /* ---- Modal DOM references ---- */
  var modalOverlay = document.getElementById("work-modal-overlay");
  var modalCloseBtn = document.getElementById("work-modal-close");
  var modalNumber = document.getElementById("modal-number");
  var modalTitle = document.getElementById("modal-title");
  var modalSubtitle = document.getElementById("modal-subtitle");
  var modalDesc = document.getElementById("modal-desc");
  var modalTechs = document.getElementById("modal-techs");
  var modalGithub = document.getElementById("modal-github");
  var modalPreview = document.getElementById("modal-preview");
  var modalPreviewImg = document.getElementById("modal-preview-img");
  var mobileBreakpoint = window.matchMedia("(max-width: 860px)");

  var activeIndex = 0;
  var isTransitioning = false;

  /* ---- Helpers ---- */

  function padNumber(n) {
    return n < 10 ? "0" + n : "" + n;
  }

  /* Build tech pill HTML */
  function buildTechPills(techs) {
    return techs
      .map(function (t) {
        return (
          '<span class="work-tech-pill">' +
          '<span class="work-tech-dot" style="background:' + t.color + '"></span>' +
          t.name +
          "</span>"
        );
      })
      .join("");
  }

  /* Swap a preview <img>, falling back to the placeholder icon when a project
     has no image. `icon` may be null (the modal has no fallback icon). */
  function setPreviewImage(imgEl, icon, src) {
    if (!imgEl) return;
    if (src) {
      imgEl.src = src;
      imgEl.style.display = "";
      if (icon) icon.style.display = "none";
    } else {
      imgEl.removeAttribute("src");
      imgEl.style.display = "none";
      if (icon) icon.style.display = "";
    }
  }

  /* ---- Render project list (left side) ---- */

  function renderList() {
    var html = "";
    getActiveProjects().forEach(function (p, i) {
      html +=
        '<button class="work-list-item' +
        (i === 0 ? " is-active" : "") +
        '" role="tab" aria-selected="' +
        (i === 0 ? "true" : "false") +
        '" data-index="' +
        i +
        '">' +
        '<span class="work-list-number">' +
        padNumber(p.id) +
        "</span>" +
        '<span class="work-list-text">' +
        '<span class="work-list-name">' +
        p.name +
        "</span>" +
        '<span class="work-list-sub">' +
        p.subtitle +
        "</span>" +
        "</span>" +
        "</button>";
    });
    listEl.innerHTML = html;
  }

  /* ---- Update detail panel (right side) ---- */

  function updateDetail(index) {
    var p = getActiveProjects()[index];
    if (!p) return;
    detailNumber.textContent = padNumber(p.id);
    detailTitle.textContent = p.name;
    detailSubtitle.textContent = p.subtitle;
    detailDesc.textContent = p.description;
    detailTechs.innerHTML = buildTechPills(p.techs);
    detailGithub.href = p.github;
    setPreviewImage(detailPreviewImg, detailPreviewIcon, p.image);
  }

  /* ---- Mobile modal helpers ---- */

  function openModal(index) {
    var p = getActiveProjects()[index];
    if (!p) return;
    modalNumber.textContent = padNumber(p.id);
    modalTitle.textContent = p.name;
    modalSubtitle.textContent = p.subtitle;
    modalDesc.textContent = p.description;
    modalTechs.innerHTML = buildTechPills(p.techs);
    modalGithub.href = p.github;
    setPreviewImage(modalPreviewImg, null, p.image);
    if (modalPreview) modalPreview.style.display = p.image ? "" : "none";

    var githubSpan = modalGithub.querySelector("[data-i18n]");
    if (githubSpan && window.i18n && window.i18n.lang() === "fa") {
      githubSpan.textContent = window.i18n.t("work.view_github", "View GitHub");
    }

    modalOverlay.classList.add("is-open");
    document.body.style.overflow = "hidden";
    modalCloseBtn.focus();
  }

  function closeModal() {
    modalOverlay.classList.remove("is-open");
    document.body.style.overflow = "";
  }

  if (modalCloseBtn) {
    modalCloseBtn.addEventListener("click", closeModal);
  }

  if (modalOverlay) {
    modalOverlay.addEventListener("click", function (e) {
      if (e.target === modalOverlay) closeModal();
    });
  }

  document.addEventListener("keydown", function (e) {
    if (e.key === "Escape" && modalOverlay && modalOverlay.classList.contains("is-open")) {
      closeModal();
    }
  });

  /* ---- Select a project with optional GSAP fade ---- */

  function selectProject(index) {
    if (index === activeIndex || isTransitioning) return;

    /* Update list highlight */
    var items = listEl.querySelectorAll(".work-list-item");
    items[activeIndex].classList.remove("is-active");
    items[activeIndex].setAttribute("aria-selected", "false");
    items[index].classList.add("is-active");
    items[index].setAttribute("aria-selected", "true");

    activeIndex = index;

    /* Animate with GSAP if available, otherwise instant swap */
    if (typeof gsap !== "undefined") {
      isTransitioning = true;
      gsap.to(detailInner, {
        opacity: 0,
        y: 8,
        duration: 0.2,
        ease: "power2.in",
        onComplete: function () {
          updateDetail(index);
          gsap.to(detailInner, {
            opacity: 1,
            y: 0,
            duration: 0.3,
            ease: "power2.out",
            onComplete: function () {
              isTransitioning = false;
            },
          });
        },
      });
    } else {
      /* CSS-only fallback */
      detailInner.classList.add("is-fading");
      setTimeout(function () {
        updateDetail(index);
        detailInner.classList.remove("is-fading");
      }, 250);
    }
  }

  /* ---- Event delegation for project list clicks ---- */

  listEl.addEventListener("click", function (e) {
    var btn = e.target.closest(".work-list-item");
    if (!btn) return;
    var idx = parseInt(btn.getAttribute("data-index"), 10);
    if (mobileBreakpoint.matches) {
      openModal(idx);
    } else {
      selectProject(idx);
    }
  });

  /* Keyboard navigation — arrow keys to move between projects */
  listEl.addEventListener("keydown", function (e) {
    if (e.key === "ArrowDown" || e.key === "ArrowRight") {
      e.preventDefault();
      var next = (activeIndex + 1) % getActiveProjects().length;
      selectProject(next);
      listEl.querySelectorAll(".work-list-item")[next].focus();
    } else if (e.key === "ArrowUp" || e.key === "ArrowLeft") {
      e.preventDefault();
      var prev = (activeIndex - 1 + getActiveProjects().length) % getActiveProjects().length;
      selectProject(prev);
      listEl.querySelectorAll(".work-list-item")[prev].focus();
    }
  });

  /* ---- Initialize ---- */

  renderList();
  updateDetail(0);

  /* ---- Expose rebuild for i18n language switching ---- */
  window.__rebuildWorkProjects = function () {
    activeIndex = 0;
    renderList();
    updateDetail(0);
    // Update the View GitHub link text
    var githubLink = document.querySelector('.work-detail-github');
    if (githubLink) {
      var span = githubLink.querySelector('[data-i18n]');
      if (span && window.i18n && window.i18n.lang() === 'fa') {
        span.textContent = window.i18n.t('work.view_github', 'View GitHub');
      }
    }
  };
})();

/* ==========================================================================
   Text reveals (uses revealText from text-reveal.js)
   ========================================================================== */
(function () {
  "use strict";

  if (typeof window.revealText !== "function") return;

  var REVEALS = [
    [".work-heading", { type: "chars", stagger: 0.025, blurAmount: 14, duration: 0.6 }],
    [".work-detail-title", { type: "chars", stagger: 0.025, blurAmount: 10, duration: 0.5 }],
    [".work-detail-subtitle", { type: "words", stagger: 0.03, blurAmount: 8, duration: 0.5 }],
    [".work-detail-desc", { type: "words", stagger: 0.03, blurAmount: 8, duration: 0.5 }],
    [".closing-cta-heading", { type: "chars", stagger: 0.02, blurAmount: 14, duration: 0.6 }],
    [".closing-cta-card-title", { type: "chars", stagger: 0.02, blurAmount: 10, duration: 0.5 }],
    [".closing-cta-card-desc", { type: "words", stagger: 0.03, blurAmount: 8, duration: 0.45 }],
  ];

  REVEALS.forEach(function (entry) {
    window.revealText(entry[0], Object.assign({ scroll: true }, entry[1]));
  });
})();
