/* ==========================================================================
   command-palette.js — Command palette modal (Ctrl/Cmd+K)
   --------------------------------------------------------------------------
   Multi-page version: reads window.__navConfig for basePath to build
   correct inter-page links. Supports i18n via window.i18n.
   ========================================================================== */

(function () {
  "use strict";

  var config = window.__navConfig || {};
  var bp = config.basePath || "../../";

  function t(key, fallback) {
    if (window.i18n) return window.i18n.t(key, fallback);
    return fallback;
  }

  function getItems() {
    var isFa = window.i18n && window.i18n.lang() === "fa";
    return [
      { label: isFa ? t("nav.home", "Home") : "Home", href: bp + "pages/home/home.html", type: "page" },
      { label: isFa ? t("nav.about", "About") : "About", href: bp + "pages/about/about.html", type: "page" },
      { label: isFa ? t("nav.experience", "Experience") : "Experience", href: bp + "pages/experience/experience.html", type: "page" },
      { label: isFa ? t("nav.work", "Work") : "Work", href: bp + "pages/work/work.html", type: "page" },
      { label: isFa ? t("nav.blog", "Blog") : "Blog", href: bp + "pages/blog/blog.html", type: "page" },
      { label: isFa ? t("nav.contact", "Contact") : "Contact", href: bp + "pages/contact/contact.html", type: "page" },
      { label: "GitHub", href: "https://github.com", type: "external" },
      { label: "LinkedIn", href: "https://linkedin.com", type: "external" },
    ];
  }

  var items = getItems();

  var overlay = document.createElement("div");
  overlay.className = "cmd-palette-overlay";
  overlay.setAttribute("role", "dialog");
  overlay.setAttribute("aria-modal", "true");
  overlay.setAttribute("aria-label", "Command palette");

  var card = document.createElement("div");
  card.className = "cmd-palette-card";

  var inputWrap = document.createElement("div");
  inputWrap.className = "cmd-palette-input-wrap";

  var searchIconSvg =
    '<svg class="cmd-palette-search-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">' +
    '<circle cx="11" cy="11" r="8"/>' +
    '<line x1="21" y1="21" x2="16.65" y2="16.65"/>' +
    "</svg>";

  inputWrap.innerHTML = searchIconSvg;

  var input = document.createElement("input");
  input.className = "cmd-palette-input";
  input.type = "text";
  input.placeholder = t("cmd.search", "Search or jump to…");
  input.setAttribute("autocomplete", "off");
  input.setAttribute("spellcheck", "false");
  inputWrap.appendChild(input);

  var list = document.createElement("ul");
  list.className = "cmd-palette-list";
  list.setAttribute("role", "listbox");

  card.appendChild(inputWrap);
  card.appendChild(list);
  overlay.appendChild(card);
  document.body.appendChild(overlay);

  function pageIcon() {
    return (
      '<svg class="cmd-palette-item-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">' +
      '<path d="M15 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V7Z"/>' +
      '<path d="M14 2v4a2 2 0 0 0 2 2h4"/>' +
      "</svg>"
    );
  }

  function externalIcon() {
    return (
      '<svg class="cmd-palette-item-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">' +
      '<path d="M18 13v6a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V8a2 2 0 0 1 2-2h6"/>' +
      '<polyline points="15 3 21 3 21 9"/>' +
      '<line x1="10" y1="14" x2="21" y2="3"/>' +
      "</svg>"
    );
  }

  var selectedIndex = 0;
  var filteredItems = [];

  function renderList(query) {
    var q = (query || "").toLowerCase().trim();

    filteredItems = items.filter(function (item) {
      return !q || item.label.toLowerCase().indexOf(q) !== -1;
    });

    if (filteredItems.length === 0) {
      list.innerHTML =
        '<li class="cmd-palette-empty">' + t("cmd.no_results", "No results found.") + '</li>';
      selectedIndex = -1;
      return;
    }

    selectedIndex = 0;
    var isFa = window.i18n && window.i18n.lang() === "fa";

    list.innerHTML = filteredItems
      .map(function (item, i) {
        var icon = item.type === "external" ? externalIcon() : pageIcon();
        var cls = "cmd-palette-item" + (i === 0 ? " selected" : "");
        var typeLabel = item.type === "external"
          ? (isFa ? t("cmd.external", "external") : "external")
          : (isFa ? t("cmd.page", "page") : "page");
        return (
          '<li class="' + cls + '" role="option" data-index="' + i + '">' +
          icon +
          '<span class="cmd-palette-item-label">' + escapeHtml(item.label) + "</span>" +
          '<span class="cmd-palette-item-type">' + typeLabel + "</span>" +
          "</li>"
        );
      })
      .join("");

    var listItems = list.querySelectorAll(".cmd-palette-item");
    listItems.forEach(function (li) {
      li.addEventListener("click", function () {
        var idx = parseInt(li.getAttribute("data-index"), 10);
        navigateTo(filteredItems[idx]);
      });
    });
  }

  function escapeHtml(str) {
    var div = document.createElement("div");
    div.appendChild(document.createTextNode(str));
    return div.innerHTML;
  }

  function updateSelection() {
    var listItems = list.querySelectorAll(".cmd-palette-item");
    listItems.forEach(function (li, i) {
      li.classList.toggle("selected", i === selectedIndex);
      if (i === selectedIndex) {
        li.scrollIntoView({ block: "nearest" });
      }
    });
  }

  function navigateTo(item) {
    if (!item) return;
    closePalette();
    if (item.type === "external") {
      window.open(item.href, "_blank", "noopener");
    } else {
      window.location.href = item.href;
    }
  }

  var triggerButton = null;
  var isOpen = false;

  function openPalette(trigger) {
    triggerButton = trigger || document.activeElement;
    isOpen = true;
    input.value = "";
    input.placeholder = t("cmd.search", "Search or jump to…");
    items = getItems();
    renderList("");
    overlay.classList.add("open");
    requestAnimationFrame(function () {
      input.focus();
    });
  }

  function closePalette() {
    isOpen = false;
    overlay.classList.remove("open");
    if (triggerButton && triggerButton.focus) {
      triggerButton.focus();
    }
    triggerButton = null;
  }

  input.addEventListener("input", function () {
    renderList(input.value);
  });

  input.addEventListener("keydown", function (e) {
    if (e.key === "ArrowDown") {
      e.preventDefault();
      if (filteredItems.length > 0) {
        selectedIndex = (selectedIndex + 1) % filteredItems.length;
        updateSelection();
      }
    } else if (e.key === "ArrowUp") {
      e.preventDefault();
      if (filteredItems.length > 0) {
        selectedIndex = (selectedIndex - 1 + filteredItems.length) % filteredItems.length;
        updateSelection();
      }
    } else if (e.key === "Enter") {
      e.preventDefault();
      if (selectedIndex >= 0 && filteredItems[selectedIndex]) {
        navigateTo(filteredItems[selectedIndex]);
      }
    } else if (e.key === "Escape") {
      e.preventDefault();
      closePalette();
    }
  });

  overlay.addEventListener("click", function (e) {
    if (e.target === overlay) closePalette();
  });

  overlay.addEventListener("keydown", function (e) {
    if (e.key === "Tab") {
      e.preventDefault();
      input.focus();
    }
  });

  document.addEventListener("keydown", function (e) {
    var isCtrlK = (e.key === "k" || e.key === "K") && (e.metaKey || e.ctrlKey);
    if (isCtrlK) {
      e.preventDefault();
      if (isOpen) closePalette();
      else openPalette();
    }
  });

  var cmdBtn = document.querySelector(".navbar-cmd-btn");
  if (cmdBtn) {
    cmdBtn.addEventListener("click", function () {
      openPalette(cmdBtn);
    });
  }

  window.commandPalette = {
    open: openPalette,
    close: closePalette,
    addItem: function (item) { items.push(item); },
  };

  window.__rebuildCommandPalette = function () {
    items = getItems();
    input.placeholder = t("cmd.search", "Search or jump to…");
  };
})();
