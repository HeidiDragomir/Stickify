import { User } from './user.js';

export type RegisterResponse = {
    user: User;
    accessToken: string;
    expireAt: string;
};
