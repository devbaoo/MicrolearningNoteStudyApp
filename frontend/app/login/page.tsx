'use client'

import { FaFacebookF, FaGoogle } from 'react-icons/fa'
import Link from "next/link";
export default function LoginPage() {
  return (
    <div className="flex items-center justify-center min-h-screen bg-gray-100">
      <div className="bg-white rounded-xl shadow-lg p-8 w-full max-w-md">
        <h2 className="text-2xl font-bold text-center mb-6">Đăng Nhập</h2>

        <form className="space-y-4">
          <input
            type="text"
            placeholder="Nhập tài khoản"
            className="w-full border border-gray-300 rounded-full px-4 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
            required
          />
          <input
            type="password"
            placeholder="Mật khẩu"
            className="w-full border border-gray-300 rounded-full px-4 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
            required
          />
          <button
            type="submit"
            className="w-full bg-red-400 hover:bg-red-500 text-white font-semibold py-2 rounded-full shadow"
          >
            Đăng Nhập
          </button>
        </form>

        <div className="text-center mt-3 text-sm text-blue-600 cursor-pointer">
          <Link href="resetpassword">🔒 Quên mật khẩu?</Link>
        </div>

        <div className="text-center mt-4 text-sm">
          Bạn chưa có tài khoản?{' '}
          <span className="text-blue-600 cursor-pointer font-semibold">
           <Link href = "/register">Đăng ký ngay</Link> 
          </span>
        </div>

        <div className="mt-5 space-y-2">
          <button className="w-full flex items-center justify-center gap-2 bg-blue-800 hover:bg-blue-900 text-white py-2 rounded-full">
            <FaFacebookF /> Login with Facebook
          </button>
          <button className="w-full flex items-center justify-center gap-2 bg-blue-500 hover:bg-blue-600 text-white py-2 rounded-full">
            <FaGoogle /> Login with Google
          </button>
        </div>
      </div>
    </div>
  )
}
