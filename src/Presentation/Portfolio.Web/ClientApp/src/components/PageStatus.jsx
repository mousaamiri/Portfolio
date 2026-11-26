import { Icon } from "../lib/icons";

// Visible loading / failure state for data-driven pages. Without it a failed
// API call rendered an empty <section>, which looked like a missing page.
export default function PageStatus({ loading, error }) {
  if (loading) {
    return (
      <div className="page-status" role="status" aria-live="polite">
        <span className="page-status-spinner" aria-hidden="true"></span>
        <p className="page-status-text">Loading content…</p>
      </div>
    );
  }

  return (
    <div className="page-status" role="alert">
      <Icon name="alert-circle" size={30} />
      <p className="page-status-title">Content is unavailable</p>
      <p className="page-status-text">
        The portfolio API could not be reached. Make sure Portfolio.API is running,
        then try again.
      </p>
      <button
        type="button"
        className="page-status-retry"
        onClick={() => window.location.reload()}
      >
        Retry
      </button>
      {error?.message ? (
        <p className="page-status-detail">{error.message}</p>
      ) : null}
    </div>
  );
}
