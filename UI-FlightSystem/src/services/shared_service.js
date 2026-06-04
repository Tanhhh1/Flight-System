import api from "../configs/axios_config";

function cleanParams(params) {
    return Object.fromEntries(
        Object.entries(params).filter(([, v]) => v !== "" && v !== null && v !== undefined)
    );
}

export function sharedService(base) {
    return {
        getAll: (params) => api.get(base, { params: cleanParams(params) }),
        getById: (id) => api.get(`${base}/${id}`),
        create: (data) => api.post(base, data),
        update: (id, data) => api.put(`${base}/${id}`, data),
        delete: (id) => api.patch(`${base}/${id}`),
    };
}