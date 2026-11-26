import { useEffect, useMemo, useRef, useState } from "react";
import { useNavigate } from "react-router-dom";
import { Icon } from "../lib/icons";
import { useI18n } from "../lib/i18n";
import { FEATURES, ROUTES } from "../lib/features";

export default function CommandPalette({ open, onClose }) {
  const navigate = useNavigate();
  const { t, lang } = useI18n();
  const [query, setQuery] = useState("");
  const [selected, setSelected] = useState(0);
  const inputRef = useRef(null);

  const items = useMemo(() => {
    const isFa = lang === "fa";
    const list = [
      { label: isFa ? t("nav.home", "Home") : "Home", href: ROUTES.home, type: "page" },
      { label: isFa ? t("nav.about", "About") : "About", href: ROUTES.about, type: "page" },
      { label: isFa ? t("nav.experience", "Experience") : "Experience", href: ROUTES.experience, type: "page" },
      { label: isFa ? t("nav.work", "Work") : "Work", href: ROUTES.work, type: "page" },
    ];
    if (FEATURES.blogEnabled) {
      list.push({ label: isFa ? t("nav.blog", "Blog") : "Blog", href: ROUTES.blog, type: "page" });
    }
    list.push(
      { label: isFa ? t("nav.contact", "Contact") : "Contact", href: ROUTES.contact, type: "page" },
      { label: "GitHub", href: "https://github.com", type: "external" },
      { label: "LinkedIn", href: "https://linkedin.com", type: "external" }
    );
    return list;
  }, [t, lang]);

  const filtered = useMemo(() => {
    const q = query.toLowerCase().trim();
    return items.filter((item) => !q || item.label.toLowerCase().includes(q));
  }, [items, query]);

  useEffect(() => {
    if (open) {
      setQuery("");
      setSelected(0);
      requestAnimationFrame(() => inputRef.current?.focus());
    }
  }, [open]);

  useEffect(() => {
    setSelected(0);
  }, [query]);

  if (!open) return null;

  const go = (item) => {
    if (!item) return;
    onClose();
    if (item.type === "external") window.open(item.href, "_blank", "noopener");
    else navigate(item.href);
  };

  const onKeyDown = (e) => {
    if (e.key === "ArrowDown") {
      e.preventDefault();
      if (filtered.length) setSelected((s) => (s + 1) % filtered.length);
    } else if (e.key === "ArrowUp") {
      e.preventDefault();
      if (filtered.length) setSelected((s) => (s - 1 + filtered.length) % filtered.length);
    } else if (e.key === "Enter") {
      e.preventDefault();
      go(filtered[selected]);
    } else if (e.key === "Escape") {
      e.preventDefault();
      onClose();
    }
  };

  return (
    <div
      className="cmd-palette-overlay open"
      role="dialog"
      aria-modal="true"
      aria-label="Command palette"
      onClick={(e) => {
        if (e.target === e.currentTarget) onClose();
      }}
    >
      <div className="cmd-palette-card">
        <div className="cmd-palette-input-wrap">
          <Icon name="search" className="cmd-palette-search-icon" size={18} />
          <input
            ref={inputRef}
            className="cmd-palette-input"
            type="text"
            placeholder={t("cmd.search", "Search or jump to…")}
            autoComplete="off"
            spellCheck="false"
            value={query}
            onChange={(e) => setQuery(e.target.value)}
            onKeyDown={onKeyDown}
          />
        </div>
        <ul className="cmd-palette-list" role="listbox">
          {filtered.length === 0 ? (
            <li className="cmd-palette-empty">{t("cmd.no_results", "No results found.")}</li>
          ) : (
            filtered.map((item, i) => (
              <li
                key={`${item.label}-${i}`}
                role="option"
                aria-selected={i === selected}
                className={`cmd-palette-item${i === selected ? " selected" : ""}`}
                onMouseEnter={() => setSelected(i)}
                onClick={() => go(item)}
              >
                <Icon
                  name={item.type === "external" ? "external-link" : "file-text"}
                  className="cmd-palette-item-icon"
                  size={18}
                />
                <span className="cmd-palette-item-label">{item.label}</span>
                <span className="cmd-palette-item-type">
                  {item.type === "external" ? t("cmd.external", "external") : t("cmd.page", "page")}
                </span>
              </li>
            ))
          )}
        </ul>
      </div>
    </div>
  );
}
