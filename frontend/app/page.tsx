import Link from "next/link";

export default function Home() {
  return (
    <div className="flex flex-col items-center justify-center min-h-screen p-8 gap-6">
      <h1 className="text-2xl font-bold">Trang chính</h1>
      <Link href="/login" className="px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600">
        Đăng nhập
      </Link>
    </div>
  );
}
