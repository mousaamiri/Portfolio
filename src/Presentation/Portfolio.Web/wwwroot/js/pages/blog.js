/* ==========================================================================
   blog.js — Blog page logic
   --------------------------------------------------------------------------
   Renders blog cards, category filters, search, and full-screen article
   modal with reading-progress bar.
   ========================================================================== */

(function () {
  "use strict";

  /* ====================================================================
     BLOG POST DATA
     ==================================================================== */
  /* ====================================================================
     BLOG POST DATA — read from the server-rendered JSON island (#blog-data),
     which Portfolio.Web populates from the real Portfolio.API articles. Falls
     back to an empty list if the island is missing/unparseable.
     ==================================================================== */
  var posts = [];
  try {
    var dataEl = document.getElementById("blog-data");
    if (dataEl && dataEl.textContent.trim()) {
      posts = JSON.parse(dataEl.textContent);
    }
  } catch (err) {
    posts = [];
  }

  /* ====================================================================
     CATEGORIES (derived from data)
     ==================================================================== */
  var categories = ["All"];
  posts.forEach(function (p) {
    if (categories.indexOf(p.category) === -1) categories.push(p.category);
  });

  /* ====================================================================
     DOM REFERENCES
     ==================================================================== */
  var gridEl = document.getElementById("blog-grid");
  var filtersEl = document.getElementById("blog-filters");
  var searchEl = document.getElementById("blog-search");

  var activeCategory = "All";

  /* ====================================================================
     HELPERS
     ==================================================================== */
  function formatDate(dateStr) {
    var d = new Date(dateStr + "T00:00:00");
    var days = ["SUN", "MON", "TUE", "WED", "THU", "FRI", "SAT"];
    var months = ["JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC"];
    return days[d.getDay()] + " " + months[d.getMonth()] + " " + String(d.getDate()).padStart(2, "0") + " " + d.getFullYear();
  }

  function formatModalDate(dateStr) {
    var d = new Date(dateStr + "T00:00:00");
    return String(d.getMonth() + 1).padStart(2, "0") + "/" + String(d.getDate()).padStart(2, "0") + "/" + d.getFullYear();
  }

  function escapeHtml(str) {
    var div = document.createElement("div");
    div.appendChild(document.createTextNode(str));
    return div.innerHTML;
  }

  /* ====================================================================
     RENDER FILTERS
     ==================================================================== */
  function renderFilters() {
    filtersEl.innerHTML = categories
      .map(function (cat) {
        var isActive = cat === activeCategory;
        return (
          '<button class="blog-filter-btn' +
          (isActive ? " is-active" : "") +
          '" role="tab" aria-selected="' +
          isActive +
          '" data-category="' +
          escapeHtml(cat) +
          '">' +
          escapeHtml(cat) +
          "</button>"
        );
      })
      .join("");
  }

  /* ====================================================================
     RENDER GRID
     ==================================================================== */
  function getFilteredPosts() {
    var query = (searchEl.value || "").toLowerCase().trim();
    return posts.filter(function (p) {
      var matchCategory = activeCategory === "All" || p.category === activeCategory;
      var matchSearch =
        !query ||
        p.title.toLowerCase().indexOf(query) !== -1 ||
        p.excerpt.toLowerCase().indexOf(query) !== -1 ||
        p.category.toLowerCase().indexOf(query) !== -1;
      return matchCategory && matchSearch;
    });
  }

  function renderGrid() {
    var filtered = getFilteredPosts();

    if (filtered.length === 0) {
      gridEl.innerHTML =
        '<div class="blog-empty">No articles match your search.</div>';
      return;
    }

    gridEl.innerHTML = filtered
      .map(function (p) {
        return (
          '<article class="blog-card" data-post-id="' + p.id + '" tabindex="0">' +
            '<div class="blog-card-top">' +
              '<span class="blog-card-meta">' + formatDate(p.date) + '</span>' +
              '<span class="blog-card-meta blog-card-meta--sep">&middot;</span>' +
              '<span class="blog-card-meta">' + p.readTime + ' MIN READ</span>' +
              '<span class="blog-card-tag">' + escapeHtml(p.category.toUpperCase()) + '</span>' +
            '</div>' +
            '<h3 class="blog-card-title">' + escapeHtml(p.title) + '</h3>' +
            '<p class="blog-card-excerpt">' + escapeHtml(p.excerpt) + '</p>' +
            '<span class="blog-card-link">Read <span class="blog-card-arrow">&rarr;</span></span>' +
          '</article>'
        );
      })
      .join("");
  }

  /* ====================================================================
     MODAL
     ==================================================================== */
  var isModalOpen = false;
  var escHandler = null;

  var MODAL_HTML =
    '<div class="blog-modal" role="dialog" aria-modal="true" aria-labelledby="blog-modal-title">' +
      '<div class="blog-modal-header">' +
        '<div class="blog-modal-header-left">' +
          '<span class="blog-modal-tag" id="blog-modal-tag"></span>' +
          '<span class="blog-modal-date" id="blog-modal-date"></span>' +
        '</div>' +
        '<button class="blog-modal-close" id="blog-modal-close" aria-label="Close article" type="button">' +
          '<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">' +
            '<line x1="18" y1="6" x2="6" y2="18"/>' +
            '<line x1="6" y1="6" x2="18" y2="18"/>' +
          '</svg>' +
        '</button>' +
      '</div>' +
      '<div class="blog-modal-progress-track">' +
        '<div class="blog-modal-progress-bar" id="blog-modal-progress"></div>' +
      '</div>' +
      '<div class="blog-modal-body" id="blog-modal-body">' +
        '<h1 class="blog-modal-title" id="blog-modal-title"></h1>' +
        '<div class="blog-modal-content" id="blog-modal-content"></div>' +
      '</div>' +
    '</div>';

  function openModal(postId) {
    var post = posts.find(function (p) { return p.id === postId; });
    if (!post || isModalOpen) return;

    var overlay = document.createElement("div");
    overlay.className = "blog-modal-overlay is-open";
    overlay.id = "blog-modal-overlay";
    overlay.setAttribute("aria-hidden", "false");
    overlay.innerHTML = MODAL_HTML;

    overlay.querySelector("#blog-modal-tag").textContent = post.category.toUpperCase();
    overlay.querySelector("#blog-modal-date").textContent = formatModalDate(post.date);
    overlay.querySelector("#blog-modal-title").textContent = post.title;
    overlay.querySelector("#blog-modal-content").innerHTML = post.content;

    document.body.appendChild(overlay);
    document.body.style.overflow = "hidden";
    isModalOpen = true;

    var body = overlay.querySelector("#blog-modal-body");
    var bar = overlay.querySelector("#blog-modal-progress");
    body.scrollTop = 0;

    body.addEventListener("scroll", function () {
      requestAnimationFrame(function () {
        var scrollTop = body.scrollTop;
        var scrollable = body.scrollHeight - body.clientHeight;
        var pct = scrollable > 0 ? (scrollTop / scrollable) * 100 : 0;
        bar.style.width = pct + "%";
      });
    }, { passive: true });

    overlay.querySelector("#blog-modal-close").addEventListener("click", closeModal);
    overlay.addEventListener("click", function (e) {
      if (e.target === overlay) closeModal();
    });

    escHandler = function (e) {
      if (e.key === "Escape") closeModal();
    };
    document.addEventListener("keydown", escHandler);
  }

  function closeModal() {
    if (!isModalOpen) return;
    isModalOpen = false;

    var overlay = document.getElementById("blog-modal-overlay");
    if (overlay && overlay.parentNode) {
      overlay.parentNode.removeChild(overlay);
    }
    document.body.style.overflow = "";

    if (escHandler) {
      document.removeEventListener("keydown", escHandler);
      escHandler = null;
    }
  }

  /* ====================================================================
     EVENT LISTENERS
     ==================================================================== */

  // Filter clicks
  filtersEl.addEventListener("click", function (e) {
    var btn = e.target.closest(".blog-filter-btn");
    if (!btn) return;
    activeCategory = btn.getAttribute("data-category");
    renderFilters();
    renderGrid();
  });

  // Search
  searchEl.addEventListener("input", function () {
    renderGrid();
  });

  // Card clicks — open modal
  gridEl.addEventListener("click", function (e) {
    var card = e.target.closest(".blog-card");
    if (!card) return;
    var postId = card.getAttribute("data-post-id");
    openModal(postId);
  });

  // Card keyboard — Enter/Space opens modal
  gridEl.addEventListener("keydown", function (e) {
    if (e.key !== "Enter" && e.key !== " ") return;
    var card = e.target.closest(".blog-card");
    if (!card) return;
    e.preventDefault();
    var postId = card.getAttribute("data-post-id");
    openModal(postId);
  });

  /* ====================================================================
     INIT
     ==================================================================== */
  renderFilters();
  renderGrid();
})();

/* ==========================================================================
   Text reveals (uses revealText from text-reveal.js)
   ========================================================================== */
(function () {
  "use strict";

  if (typeof window.revealText !== "function") return;

  var REVEALS = [
    [".blog-heading", { type: "chars", stagger: 0.025, blurAmount: 14, duration: 0.6 }],
  ];

  REVEALS.forEach(function (entry) {
    window.revealText(entry[0], Object.assign({ scroll: true }, entry[1]));
  });
})();
