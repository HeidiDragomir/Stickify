/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        "./src/**/*.{js,ts,jsx,tsx,mdx}",
        // "./components/**/*.{js,ts,jsx,tsx,mdx}",
        // "./app/**/*.{js,ts,jsx,tsx,mdx}",
    ],
    theme: {
        screens: {
            xs: "480px",
            // => @media (min-width: 480px) { ... }
            sm: "640px",
            // => @media (min-width: 640px) { ... }
            md: "768px",
            // => @media (min-width: 768px) { ... }
            lg: "1024px",
            // => @media (min-width: 1024px) { ... }
            xl: "1280px",
            // => @media (min-width: 1280px) { ... }
            "2xl": "1536px",
            // => @media (min-width: 1536px) { ... }
        },
        colors: {
            label: "#0a0049",
            purple: {
                DEFAULT: "#c2a6f7",
                // 100: "#dcfce7",
                // 200: "#a6f7c2",
                // dark: "#356737",
                // darker: "#253D26",
                // faded: "#6DA36F",
                // cta: "#39a23c",
            },
            orange: {
                DEFAULT: "#f7c2a6",
            },
            green: {
                DEFAULT: "#a6f7c2",
            },
            // primary, secondary, success, important, warning, danger, light, dark
        },
    },
    plugins: [],
};
