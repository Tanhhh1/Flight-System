import http from "./http";

function cleanParams(params = {}) {
    return Object.fromEntries(
        Object.entries(params).filter( ([, value]) => value !== "" && value !== null && value !== undefined )
    );
}

export const createCrudApi = (baseUrl) => ({
    getAll(params) { return http.get(baseUrl, { params: cleanParams(params)}) },
    getById(id) { return http.get(`${baseUrl}/${id}`) },
    create(data) { return http.post(baseUrl, data) },
    update(id, data) { return http.put(`${baseUrl}/${id}`, data) },
    delete(id) { return http.patch(`${baseUrl}/${id}`) },
});