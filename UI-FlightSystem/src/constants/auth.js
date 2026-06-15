export const ROLES = {
    ADMIN: "admin",
    STAFF: "staff",
    USER: "user",
};

export const ADMIN_ROLES = [ROLES.ADMIN, ROLES.STAFF];
export const CLIENT_ROLES = [ROLES.USER];

export const isAdminRole = (roles = []) => roles.some((r) => ADMIN_ROLES.includes(r));
export const isClientRole = (roles = []) => roles.some((r) => CLIENT_ROLES.includes(r));
