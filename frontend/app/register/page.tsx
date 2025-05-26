'use client'

import { FaFacebookF, FaGoogle } from 'react-icons/fa'
import Link from "next/link";
export default function RegisterPage() {
  return (
    <div className="flex items-center justify-center min-h-screen bg-gray-100">
      <div className="bg-white rounded-xl shadow-lg p-8 w-full max-w-xl">
        <h2 className="text-2xl font-bold text-center mb-6">Đăng Ký Tài Khoản</h2>

        <form className="space-y-4">
          <input
            type="text"
            placeholder="Nhập tài khoản "
            className="w-full border border-gray-300 rounded-full px-4 py-2"
            required
          />

          <div className="flex gap-4">
            <input
              type="text"
              placeholder="Họ"
              className="w-full border border-gray-300 rounded-full px-4 py-2"
              required
            />
            <input
              type="text"
              placeholder="Tên"
              className="w-full border border-gray-300 rounded-full px-4 py-2"
              required
            />
          </div>

          <div className="flex gap-4">
            <input
              type="tel"
              placeholder="Số điện thoại"
              className="w-full border border-gray-300 rounded-full px-4 py-2"
              required
            />
            <select
              className="w-full border border-gray-300 rounded-full px-4 py-2 text-gray-500"
              required
            >
              <option value="">Giới tính</option>
              <option value="Nam">Nam</option>
              <option value="Nữ">Nữ</option>
              <option value="Khác">Khác</option>
            </select>
          </div>

          <div className="flex gap-4">
            <input
              type="password"
              placeholder="Mật khẩu"
              className="w-full border border-gray-300 rounded-full px-4 py-2"
              required
            />
            <input
              type="password"
              placeholder="Nhập lại mật khẩu"
              className="w-full border border-gray-300 rounded-full px-4 py-2"
              required
            />
          </div>

          <p className="text-sm text-gray-500 text-center">
            (8 ký tự trở lên gồm cả chữ, số và ký tự đặc biệt)
          </p>

          <button
            type="submit"
            className="w-full bg-blue-500 hover:bg-blue-600 text-white font-semibold py-2 rounded-full"
          >
            Đăng Ký
          </button>
        </form>

        <div className="text-center mt-4 text-sm">
          Bạn đã có tài khoản?{' '}
          <span className="text-blue-600 font-semibold cursor-pointer">
            <Link href="login">Đăng nhập</Link>
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
