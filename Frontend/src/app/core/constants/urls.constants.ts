import { AUTHENTICATION_PATHS, ROOT_PATHS } from './paths.constants.js';

export const ROOT_URLS = {
    base: `/${ROOT_PATHS.base}`,
    notFound: `/${ROOT_PATHS.notFound}`,
};

export const AUTH_URLS = {
    login: `/${AUTHENTICATION_PATHS.login}`,
    register: `/${AUTHENTICATION_PATHS.register}`,
};
