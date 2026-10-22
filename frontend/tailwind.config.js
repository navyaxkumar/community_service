/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      fontFamily: {
        sans: ['Inter', 'system-ui', '-apple-system', 'sans-serif'],
        serif: ['"P22 Mackinac W01 Book"', 'Georgia', 'serif'],
      },
      colors: {
        shield: {
          bg: '#FFFFFF',
          dark: '#191919',
          row: '#F4F3F3',
          rowHover: '#EAEAEA',
        },
      },
    },
  },
  plugins: [],
};
