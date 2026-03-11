// wrapper around auth/other services that deal with account data
import authService from "./authService";

const accountService = {
  /**
   * Fetches the role for a given user. The backend should return a simple
   * string such as "Host" or "User" (or whatever roles your API supports).
   */
  getUserRole: async (userId) => {
    return authService.getUserRole(userId);
  },
};

export default accountService;
