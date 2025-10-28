import { User } from './user.js';

export type LoginResponse = {
  user: User;
  accessToken: string;
  expireAt: string;
};
