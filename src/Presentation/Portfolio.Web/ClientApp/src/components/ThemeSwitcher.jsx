import { useEffect, useRef, useState } from "react";
import { Icon } from "../lib/icons";
import { useI18n } from "../lib/i18n";
import { useTheme } from "../lib/theme";

export default function ThemeSwitcher() {
  const { accents, accent, setAccent } = useTheme();
  const { t } = useI18n();
  const [open, setOpen] = useState(false);
  const wrapperRef = useRef(null);

  useEffect(() => {
    const onClick = (e) => {
      if (wrapperRef.current && !wrapperRef.current.contains(e.target)) setOpen(false);
    };
    const onKey = (e) => {
      if (e.key === "Escape") setOpen(false);
    };
    document.addEventListener("click", onClick);
    document.addEventListener("keydown", onKey);
    return () => {
      document.removeEventListener("click", onClick);
      document.removeEventListener("keydown", onKey);
    };
  }, []);

  return (
    <div className={`theme-switcher${open ? " open" : ""}`} aria-label="Accent color picker" ref={wrapperRef}>
      <button
        className="theme-switcher-btn"
        type="button"
        aria-expanded={open}
        aria-haspopup="true"
        title="Change accent color"
        onClick={(e) => {
          e.stopPropagation();
          setOpen((v) => !v);
        }}
      >
        <Icon name="palette" size={20} />
      </button>
      <div className="theme-switcher-popover" role="menu">
        <span className="theme-switcher-popover-title">{t("theme.accent_color", "Accent Color")}</span>
        <div className="theme-switcher-colors">
          {accents.map((item) => (
            <button
              key={item.name}
              type="button"
              role="menuitem"
              className={`theme-color-swatch${accent === item.name ? " active" : ""}`}
              style={{ background: item.color }}
              aria-label={`${item.name} accent`}
              onClick={() => {
                setAccent(item.name);
                setOpen(false);
              }}
            />
          ))}
        </div>
      </div>
    </div>
  );
}
