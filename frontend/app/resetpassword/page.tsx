'use client'
import Link from "next/link";

export default function ForgotPasswordPage() {
  return (
    <div className="flex items-center justify-center min-h-screen bg-gradient-to-br from-blue-100 via-purple-100 to-pink-100">
      <div className="bg-white p-8 rounded-xl shadow-md w-full max-w-sm">
        {/* Logo + tên */}
        <div className="flex items-center justify-center mb-6">
        </div>

        <h2 className="text-xl font-bold text-center mb-2">Quên mật khẩu?</h2>
        <p className="text-sm text-center text-gray-600 mb-6">
          Điền email gắn với tài khoản của bạn để nhận đường dẫn thay đổi mật khẩu
        </p>

        <form className="space-y-4">
          <label className="block text-sm font-medium text-gray-700">Email</label>
          <input
            type="email"
            className="w-full border border-purple-400 rounded-md px-4 py-2 focus:outline-none focus:ring-2 focus:ring-purple-500"
            required
          />

          <button
            type="submit"
            className="w-full bg-purple-600 hover:bg-purple-700 text-white font-semibold py-2 rounded-md"
          >
            Tiếp tục
          </button>
        </form>

        <div className="mt-4 text-center">
          <a href="/login" className="text-sm text-purple-700 hover:underline">
            <Link href="login">Quay lại đăng nhập</Link>
          </a>
        </div>
      </div>
    </div>
  )
}
