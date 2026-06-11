/** @type {import('tailwindcss').Config} */
export default {
  content: ['./index.html', './src/**/*.{js,jsx,ts,tsx}'],
  theme: {
    extend: {
      fontFamily: {
        sans: ['"Heebo"', '"Segoe UI"', 'sans-serif'],
        display: ['"Fredoka"', '"Heebo"', 'sans-serif'],
      },
      colors: {
        brand: {
          50:  '#fff7ed',
          100: '#ffedd5',
          400: '#fb923c',
          500: '#f97316',
          600: '#ea580c',
        },
        sky2: {
          400: '#38bdf8',
          500: '#0ea5e9',
        },
      },
      boxShadow: {
        kid: '0 10px 25px -5px rgba(249, 115, 22, 0.35)',
      },
      borderRadius: {
        kid: '1.5rem',
      },
    },
  },
  plugins: [],
}
