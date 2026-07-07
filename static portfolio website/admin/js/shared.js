/* ==========================================================================
   shared.js — Reusable admin components: sidebar, CRUD framework, modal,
   table, search, toast, bilingual form helpers
   ========================================================================== */

/* ================= UTILITIES ================= */

var AdminUtil = {
  escapeHtml: function (str) {
    if (!str) return "";
    return String(str).replace(/&/g,"&amp;").replace(/</g,"&lt;").replace(/>/g,"&gt;").replace(/"/g,"&quot;");
  },
  truncate: function (str, n) {
    if (!str) return "";
    return str.length > n ? str.substring(0, n) + "…" : str;
  },
  debounce: function (fn, ms) {
    var t;
    return function () {
      var ctx = this, args = arguments;
      clearTimeout(t);
      t = setTimeout(function () { fn.apply(ctx, args); }, ms);
    };
  }
};

/* ================= TOAST ================= */

var AdminToast = (function () {
  function ensure() {
    var c = document.getElementById("toastContainer");
    if (c) return c;
    c = document.createElement("div");
    c.id = "toastContainer";
    c.className = "admin-toast-container";
    document.body.appendChild(c);
    return c;
  }
  return {
    show: function (msg, type) {
      var c = ensure();
      var icon = type === "success"
        ? '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"/><path d="m9 11 3 3L22 4"/></svg>'
        : '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="12" r="10"/><line x1="15" y1="9" x2="9" y2="15"/><line x1="9" y1="9" x2="15" y2="15"/></svg>';
      var el = document.createElement("div");
      el.className = "admin-toast admin-toast--" + (type || "success");
      el.innerHTML = icon + '<span>' + AdminUtil.escapeHtml(msg) + '</span>';
      c.appendChild(el);
      setTimeout(function () {
        el.classList.add("removing");
        setTimeout(function () { if (el.parentNode) el.parentNode.removeChild(el); }, 250);
      }, 3000);
    }
  };
})();

/* ================= MODAL ================= */

var AdminModal = (function () {
  var _overlay, _content;

  function getOverlay() {
    if (_overlay) return _overlay;
    _overlay = document.getElementById("modalOverlay");
    _content = document.getElementById("modalContent");
    if (!_overlay) {
      _overlay = document.createElement("div");
      _overlay.id = "modalOverlay";
      _overlay.className = "admin-modal-overlay";
      _content = document.createElement("div");
      _content.id = "modalContent";
      _content.className = "admin-modal";
      _overlay.appendChild(_content);
      document.body.appendChild(_overlay);
    }
    _overlay.addEventListener("click", function (e) {
      if (e.target === _overlay) AdminModal.close();
    });
    return _overlay;
  }

  return {
    open: function (html, opts) {
      var ov = getOverlay();
      _content.className = "admin-modal" + (opts && opts.small ? " admin-modal--sm" : "");
      _content.innerHTML = html;
      ov.classList.add("is-open");
      document.body.style.overflow = "hidden";
      if (window.lucide) lucide.createIcons({ nodes: [_content] });
      var closeBtn = _content.querySelector(".admin-modal-close");
      if (closeBtn) closeBtn.addEventListener("click", AdminModal.close);
      var cancelBtns = _content.querySelectorAll("[data-modal-cancel]");
      cancelBtns.forEach(function (b) { b.addEventListener("click", AdminModal.close); });
    },
    close: function () {
      if (_overlay) {
        _overlay.classList.remove("is-open");
        document.body.style.overflow = "";
      }
    },
    confirmDelete: function (itemName, onConfirm) {
      var html = '';
      html += '<div class="admin-modal-header"><h3 class="admin-modal-title">Delete Confirmation</h3>';
      html += '<button class="admin-modal-close"><i data-lucide="x"></i></button></div>';
      html += '<div class="admin-modal-body">';
      html += '<div class="admin-modal-icon admin-modal-icon--danger"><i data-lucide="trash-2"></i></div>';
      html += '<p class="admin-modal-msg">Are you sure you want to delete <strong>' + AdminUtil.escapeHtml(itemName) + '</strong>? This action cannot be undone.</p>';
      html += '</div>';
      html += '<div class="admin-modal-footer">';
      html += '<button class="admin-btn admin-btn--ghost" data-modal-cancel>Cancel</button>';
      html += '<button class="admin-btn admin-btn--danger" id="confirmDeleteBtn"><i data-lucide="trash-2"></i> Delete</button>';
      html += '</div>';
      AdminModal.open(html, { small: true });
      document.getElementById("confirmDeleteBtn").addEventListener("click", function () {
        AdminModal.close();
        onConfirm();
      });
    }
  };
})();

/* ================= SIDEBAR ================= */

var AdminSidebar = {
  NAV: [
    { page: "index", label: "Dashboard", icon: "layout-dashboard", href: "index.html" },
    { page: "projects", label: "Projects", icon: "folder-kanban", href: "projects.html" },
    { page: "articles", label: "Articles", icon: "file-text", href: "articles.html" },
    { page: "experiences", label: "Experiences", icon: "briefcase", href: "experiences.html" },
    { page: "education", label: "Education", icon: "graduation-cap", href: "education.html" },
    { page: "skills", label: "Skills", icon: "star", href: "skills.html" },
    { page: "testimonials", label: "Testimonials", icon: "quote", href: "testimonials.html" },
    { page: "messages", label: "Messages", icon: "mail", href: "messages.html", badge: true }
  ],

  init: function (activePage) {
    var sidebar = document.getElementById("adminSidebar");
    if (!sidebar) return;

    var navHtml = '';
    AdminSidebar.NAV.forEach(function (item) {
      var active = item.page === activePage ? " is-active" : "";
      var badge = "";
      if (item.badge) {
        var count = AdminData.unreadMessages();
        if (count > 0) badge = '<span class="admin-nav-badge">' + count + '</span>';
      }
      navHtml += '<a href="' + item.href + '" class="admin-nav-link' + active + '">';
      navHtml += '<i data-lucide="' + item.icon + '" class="admin-nav-icon"></i>';
      navHtml += '<span>' + item.label + '</span>' + badge + '</a>';
    });

    sidebar.innerHTML = '<div class="admin-sidebar-header">' +
      '<img id="sidebarLogo" src="../assets/logo-en-dark.svg" alt="Logo" class="admin-sidebar-logo" />' +
      '<span class="admin-sidebar-brand">Admin</span></div>' +
      '<nav class="admin-nav">' + navHtml + '</nav>' +
      '<div class="admin-sidebar-footer">' +
      '<a href="../pages/home/home.html" class="admin-nav-link admin-nav-link--subtle" target="_blank">' +
      '<i data-lucide="external-link" class="admin-nav-icon"></i><span>View Site</span></a>' +
      '<button class="admin-nav-link admin-nav-link--subtle" id="themeToggleBtn">' +
      '<span id="themeIconLight" class="admin-theme-icon"><i data-lucide="sun" class="admin-nav-icon"></i></span>' +
      '<span id="themeIconDark" class="admin-theme-icon"><i data-lucide="moon" class="admin-nav-icon"></i></span>' +
      '<span id="themeToggleText">Light Mode</span></button>' +
      '<button class="admin-nav-link admin-nav-link--subtle" id="logoutBtn">' +
      '<i data-lucide="log-out" class="admin-nav-icon"></i><span>Logout</span></button></div>';

    // Theme toggle
    AdminSidebar._initTheme();
    // Mobile hamburger
    AdminSidebar._initMobile();
    // Logout
    var logoutBtn = document.getElementById("logoutBtn");
    if (logoutBtn) {
      logoutBtn.addEventListener("click", function () {
        try { sessionStorage.removeItem("admin_auth"); } catch(e) {}
        window.location.href = "login.html";
      });
    }
  },

  _initTheme: function () {
    var btn = document.getElementById("themeToggleBtn");
    if (!btn) return;
    function update() {
      var isDark = document.documentElement.getAttribute("data-theme") !== "light";
      document.getElementById("themeIconLight").style.display = isDark ? "inline-flex" : "none";
      document.getElementById("themeIconDark").style.display = isDark ? "none" : "inline-flex";
      document.getElementById("themeToggleText").textContent = isDark ? "Light Mode" : "Dark Mode";
      var logo = document.getElementById("sidebarLogo");
      if (logo) logo.src = "../assets/logo-en-" + (isDark ? "dark" : "light") + ".svg";
    }
    update();
    btn.addEventListener("click", function () {
      var cur = document.documentElement.getAttribute("data-theme") || "dark";
      var next = cur === "light" ? "dark" : "light";
      document.documentElement.setAttribute("data-theme", next);
      try { localStorage.setItem("portfolio-theme", next); } catch (e) {}
      update();
    });
  },

  _initMobile: function () {
    var hamburger = document.getElementById("hamburgerBtn");
    var sidebar = document.getElementById("adminSidebar");
    var overlay = document.getElementById("sidebarOverlay");
    if (!hamburger || !sidebar) return;
    function close() {
      sidebar.classList.remove("is-open");
      if (overlay) overlay.classList.remove("is-visible");
    }
    hamburger.addEventListener("click", function () {
      sidebar.classList.toggle("is-open");
      if (overlay) overlay.classList.toggle("is-visible");
    });
    if (overlay) overlay.addEventListener("click", close);
    sidebar.addEventListener("click", function (e) {
      if (e.target.closest(".admin-nav-link") && window.innerWidth < 768) close();
    });
  }
};

/* ================= CRUD FRAMEWORK ================= */

var AdminCRUD = (function () {

  var _cfg = null;

  function init(config) {
    _cfg = config;
    render();
  }

  /* ---- Render list ---- */
  function render() {
    var container = document.getElementById("crudContent");
    if (!container) return;

    var searchKeys = _cfg.searchKeys || [];
    var searchVal = "";
    var searchInput = document.getElementById("crudSearch");
    if (searchInput) searchVal = searchInput.value;

    var items = _cfg.store.getAll(searchVal, searchKeys);
    var html = "";

    // Page header
    html += '<div class="admin-page-header"><div>';
    html += '<h1 class="admin-page-title">' + AdminUtil.escapeHtml(_cfg.title) + '</h1>';
    if (_cfg.subtitle) html += '<p class="admin-page-subtitle">' + AdminUtil.escapeHtml(_cfg.subtitle) + '</p>';
    html += '</div>';
    if (_cfg.canAdd !== false) {
      html += '<button class="admin-btn admin-btn--primary" id="crudAddBtn">';
      html += '<i data-lucide="plus"></i> Add New</button>';
    }
    html += '</div>';

    // Table
    html += '<div class="admin-table-wrap">';

    // Toolbar
    html += '<div class="admin-table-toolbar">';
    html += '<div class="admin-search-wrap">';
    html += '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="11" cy="11" r="8"/><path d="m21 21-4.3-4.3"/></svg>';
    html += '<input type="text" class="admin-search-input" id="crudSearch" placeholder="Search..." value="' + AdminUtil.escapeHtml(searchVal) + '" />';
    html += '</div></div>';

    if (items.length === 0) {
      html += '<div class="admin-empty">';
      html += '<i data-lucide="inbox"></i>';
      html += '<p class="admin-empty-text">No items found</p>';
      html += '<p class="admin-empty-sub">Try adjusting your search or add a new item</p>';
      html += '</div>';
    } else {
      html += '<table class="admin-table"><thead><tr>';
      _cfg.columns.forEach(function (col) {
        var style = col.width ? ' style="width:' + col.width + '"' : "";
        html += '<th' + style + '>' + AdminUtil.escapeHtml(col.label) + '</th>';
      });
      html += '<th style="width:100px">Actions</th></tr></thead><tbody>';

      items.forEach(function (item) {
        html += '<tr>';
        _cfg.columns.forEach(function (col) {
          var val = col.render ? col.render(item) : AdminUtil.escapeHtml(item[col.key] || "");
          html += '<td data-label="' + AdminUtil.escapeHtml(col.label) + '">' + val + '</td>';
        });
        html += '<td data-label="Actions"><div class="admin-table-actions">';
        if (_cfg.canAdd !== false) {
          html += '<button class="admin-btn admin-btn--ghost admin-btn--sm" data-crud-edit="' + item.id + '"><i data-lucide="pencil"></i> Edit</button>';
        }
        if (_cfg.onView) {
          html += '<button class="admin-btn admin-btn--ghost admin-btn--sm" data-crud-view="' + item.id + '"><i data-lucide="eye"></i></button>';
        }
        html += '<button class="admin-btn admin-btn--danger admin-btn--sm admin-btn--icon" data-crud-del="' + item.id + '" title="Delete"><i data-lucide="trash-2"></i></button>';
        html += '</div></td></tr>';
      });

      html += '</tbody></table>';
    }
    html += '</div>';

    container.innerHTML = html;
    if (window.lucide) lucide.createIcons({ nodes: [container] });
    bindEvents();
  }

  /* ---- Bind events ---- */
  function bindEvents() {
    var search = document.getElementById("crudSearch");
    if (search) {
      search.addEventListener("input", AdminUtil.debounce(function () { render(); }, 300));
      search.focus();
      var v = search.value;
      search.value = "";
      search.value = v;
    }

    var addBtn = document.getElementById("crudAddBtn");
    if (addBtn) {
      addBtn.addEventListener("click", function () { openForm(null); });
    }

    document.querySelectorAll("[data-crud-edit]").forEach(function (btn) {
      btn.addEventListener("click", function () {
        var item = _cfg.store.getById(btn.dataset.crudEdit);
        if (item) openForm(item);
      });
    });

    document.querySelectorAll("[data-crud-del]").forEach(function (btn) {
      btn.addEventListener("click", function () {
        var item = _cfg.store.getById(btn.dataset.crudDel);
        var name = item ? (item.title_en || item.name_en || item.jobTitle_en || item.institution_en || item.subject || item.name || "this item") : "this item";
        AdminModal.confirmDelete(name, function () {
          _cfg.store.remove(btn.dataset.crudDel);
          AdminToast.show("Item deleted", "success");
          render();
        });
      });
    });

    document.querySelectorAll("[data-crud-view]").forEach(function (btn) {
      btn.addEventListener("click", function () {
        var item = _cfg.store.getById(btn.dataset.crudView);
        if (item && _cfg.onView) _cfg.onView(item);
      });
    });
  }

  /* ---- Open form modal ---- */
  function openForm(item) {
    var isEdit = !!item;
    var title = isEdit ? ("Edit " + _cfg.entityName) : ("Add " + _cfg.entityName);

    var html = '';
    html += '<div class="admin-modal-header"><h3 class="admin-modal-title">' + AdminUtil.escapeHtml(title) + '</h3>';
    html += '<button class="admin-modal-close"><i data-lucide="x"></i></button></div>';
    html += '<div class="admin-modal-body"><form class="admin-form" id="crudForm">';

    _cfg.formFields.forEach(function (field) {
      if (field.type === "row") {
        html += '<div class="admin-form-row">';
        field.fields.forEach(function (f) { html += renderField(f, item); });
        html += '</div>';
        return;
      }
      html += renderField(field, item);
    });

    html += '</form></div>';
    html += '<div class="admin-modal-footer">';
    html += '<button class="admin-btn admin-btn--ghost" data-modal-cancel>Cancel</button>';
    html += '<button class="admin-btn admin-btn--primary" id="crudSaveBtn"><i data-lucide="check"></i> ' + (isEdit ? "Save" : "Create") + '</button>';
    html += '</div>';

    AdminModal.open(html);

    // Init tag inputs
    _cfg.formFields.forEach(function (field) {
      var fields = field.type === "row" ? field.fields : [field];
      fields.forEach(function (f) {
        if (f.type === "tags") initTagInput(f, item);
      });
    });

    // Save
    document.getElementById("crudSaveBtn").addEventListener("click", function () {
      var form = document.getElementById("crudForm");
      var data = collectFormData();
      if (!data) return;

      if (isEdit) {
        _cfg.store.update(item.id, data);
        AdminToast.show(_cfg.entityName + " updated", "success");
      } else {
        _cfg.store.add(data);
        AdminToast.show(_cfg.entityName + " created", "success");
      }
      AdminModal.close();
      render();
    });
  }

  /* ---- Render a single form field ---- */
  function renderField(f, item) {
    var html = "";

    if (f.bilingual) {
      html += '<div class="admin-bilingual-group">';
      html += '<label class="admin-form-label">' + AdminUtil.escapeHtml(f.label) + '</label>';
      html += '<div class="admin-bilingual-pair">';

      // EN
      html += '<div class="admin-form-group">';
      html += '<label class="admin-form-label"><span class="admin-lang-badge admin-lang-badge--en">EN</span></label>';
      if (f.type === "textarea") {
        html += '<textarea class="admin-form-textarea" name="' + f.key + '_en" placeholder="' + AdminUtil.escapeHtml(f.placeholder || "") + ' (English)" rows="' + (f.rows || 3) + '">' + AdminUtil.escapeHtml(item ? item[f.key + "_en"] : "") + '</textarea>';
      } else {
        html += '<input type="text" class="admin-form-input" name="' + f.key + '_en" placeholder="' + AdminUtil.escapeHtml(f.placeholder || "") + ' (English)" value="' + AdminUtil.escapeHtml(item ? item[f.key + "_en"] : "") + '"' + (f.required ? " required" : "") + ' />';
      }
      html += '</div>';

      // FA
      html += '<div class="admin-form-group">';
      html += '<label class="admin-form-label"><span class="admin-lang-badge admin-lang-badge--fa">FA</span></label>';
      if (f.type === "textarea") {
        html += '<textarea class="admin-form-textarea admin-form-input--rtl" name="' + f.key + '_fa" placeholder="' + AdminUtil.escapeHtml(f.placeholderFa || f.placeholder || "") + ' (فارسی)" rows="' + (f.rows || 3) + '">' + AdminUtil.escapeHtml(item ? item[f.key + "_fa"] : "") + '</textarea>';
      } else {
        html += '<input type="text" class="admin-form-input admin-form-input--rtl" name="' + f.key + '_fa" placeholder="' + AdminUtil.escapeHtml(f.placeholderFa || f.placeholder || "") + ' (فارسی)" value="' + AdminUtil.escapeHtml(item ? item[f.key + "_fa"] : "") + '" />';
      }
      html += '</div>';

      html += '</div></div>';
      return html;
    }

    html += '<div class="admin-form-group">';
    html += '<label class="admin-form-label">' + AdminUtil.escapeHtml(f.label) + '</label>';

    var val = item ? (item[f.key] != null ? item[f.key] : "") : (f.defaultValue != null ? f.defaultValue : "");

    switch (f.type) {
      case "textarea":
        html += '<textarea class="admin-form-textarea" name="' + f.key + '" placeholder="' + AdminUtil.escapeHtml(f.placeholder || "") + '" rows="' + (f.rows || 3) + '">' + AdminUtil.escapeHtml(val) + '</textarea>';
        break;
      case "select":
        html += '<select class="admin-form-select" name="' + f.key + '">';
        (f.options || []).forEach(function (opt) {
          var optVal = typeof opt === "object" ? opt.value : opt;
          var optLabel = typeof opt === "object" ? opt.label : opt;
          html += '<option value="' + AdminUtil.escapeHtml(optVal) + '"' + (String(val) === String(optVal) ? " selected" : "") + '>' + AdminUtil.escapeHtml(optLabel) + '</option>';
        });
        html += '</select>';
        break;
      case "checkbox":
        html += '<label class="admin-checkbox-label"><input type="checkbox" class="admin-checkbox" name="' + f.key + '"' + (val ? " checked" : "") + ' /> ' + AdminUtil.escapeHtml(f.checkboxLabel || f.label) + '</label>';
        break;
      case "number":
        html += '<input type="number" class="admin-form-input" name="' + f.key + '" placeholder="' + AdminUtil.escapeHtml(f.placeholder || "") + '" value="' + AdminUtil.escapeHtml(val) + '" min="' + (f.min != null ? f.min : "") + '" max="' + (f.max != null ? f.max : "") + '" />';
        break;
      case "tags":
        html += '<div class="admin-tag-input-wrap" id="tagWrap_' + f.key + '"></div>';
        if (f.hint) html += '<span class="admin-form-hint">' + AdminUtil.escapeHtml(f.hint) + '</span>';
        break;
      default:
        html += '<input type="' + (f.type || "text") + '" class="admin-form-input" name="' + f.key + '" placeholder="' + AdminUtil.escapeHtml(f.placeholder || "") + '" value="' + AdminUtil.escapeHtml(val) + '"' + (f.required ? " required" : "") + ' />';
    }

    html += '</div>';
    return html;
  }

  /* ---- Tag input ---- */
  var _tagInstances = {};

  function initTagInput(f, item) {
    var wrap = document.getElementById("tagWrap_" + f.key);
    if (!wrap) return;
    var tags = item ? (item[f.key] || []).slice() : [];

    function renderTags() {
      var html = "";
      tags.forEach(function (tag, i) {
        html += '<span class="admin-tag">' + AdminUtil.escapeHtml(tag) +
          '<button type="button" class="admin-tag-remove" data-tag-idx="' + i + '">&times;</button></span>';
      });
      html += '<input type="text" class="admin-tag-field" placeholder="' + AdminUtil.escapeHtml(f.placeholder || "Type and press Enter") + '" />';
      wrap.innerHTML = html;

      wrap.querySelectorAll(".admin-tag-remove").forEach(function (btn) {
        btn.addEventListener("click", function () {
          tags.splice(parseInt(btn.dataset.tagIdx), 1);
          renderTags();
        });
      });

      var input = wrap.querySelector(".admin-tag-field");
      input.addEventListener("keydown", function (e) {
        if (e.key === "Enter" || e.key === ",") {
          e.preventDefault();
          var v = input.value.trim().replace(/,$/,"");
          if (v && tags.indexOf(v) === -1) { tags.push(v); renderTags(); }
        }
        if (e.key === "Backspace" && !input.value && tags.length) {
          tags.pop(); renderTags();
        }
      });

      wrap.addEventListener("click", function () { wrap.querySelector(".admin-tag-field").focus(); });
    }

    renderTags();
    _tagInstances[f.key] = { getTags: function () { return tags.slice(); } };
  }

  /* ---- Collect form data ---- */
  function collectFormData() {
    var data = {};
    var valid = true;

    _cfg.formFields.forEach(function (field) {
      var fields = field.type === "row" ? field.fields : [field];
      fields.forEach(function (f) {
        if (f.bilingual) {
          data[f.key + "_en"] = getVal(f.key + "_en");
          data[f.key + "_fa"] = getVal(f.key + "_fa");
          if (f.required && !data[f.key + "_en"]) {
            valid = false;
            AdminToast.show(f.label + " (EN) is required", "error");
          }
        } else if (f.type === "tags") {
          data[f.key] = _tagInstances[f.key] ? _tagInstances[f.key].getTags() : [];
        } else if (f.type === "checkbox") {
          var cb = document.querySelector('[name="' + f.key + '"]');
          data[f.key] = cb ? cb.checked : false;
        } else if (f.type === "number") {
          var nv = getVal(f.key);
          data[f.key] = nv ? Number(nv) : 0;
        } else {
          data[f.key] = getVal(f.key);
          if (f.required && !data[f.key]) {
            valid = false;
            AdminToast.show(f.label + " is required", "error");
          }
        }
      });
    });

    return valid ? data : null;
  }

  function getVal(name) {
    var el = document.querySelector('[name="' + name + '"]');
    return el ? el.value.trim() : "";
  }

  return { init: init, render: render };
})();
