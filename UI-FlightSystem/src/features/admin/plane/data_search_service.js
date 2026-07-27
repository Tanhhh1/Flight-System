import http from "@/api/http";

export const dataSearchService = {
    get: (includes) => http.get("/DataSearch", {
        params: { Include: includes },
        paramsSerializer: (params) => {
            return params.Include
                .map((v) => `Include=${v}`)
                .join("&");
        },
    }),
};

export const DataSearch = {
    Airports: 0,
    Airlines: 1,
    Services: 2,
    Planes: 3,
    Routes: 4,
    PassengerType: 5
};