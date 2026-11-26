import * as Lucide from "lucide-react";

// Renders a lucide icon by its kebab-case name (e.g. "graduation-cap"),
// matching the names used across the original Razor markup / data-lucide attrs.
function toPascal(name) {
  return String(name || "")
    .split(/[-_\s]+/)
    .filter(Boolean)
    .map((part) => part.charAt(0).toUpperCase() + part.slice(1))
    .join("");
}

export function Icon({ name, size, strokeWidth, ...rest }) {
  const Component = Lucide[toPascal(name)] || Lucide.Circle;
  return <Component size={size} strokeWidth={strokeWidth} {...rest} />;
}
