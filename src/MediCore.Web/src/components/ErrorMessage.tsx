export default function ErrorMessage({ message }: { message: string | null }) {
  return message ? <div className="alert alert--error" role="alert">{message}</div> : null;
}
