export const getEndpoints = () => {
    const API_URL = 'https://localhost:7251/api';

    return {
        auth: {
            login: `${API_URL}/auth/login`,
            register: `${API_URL}/auth/register`,
        },
    };
};
