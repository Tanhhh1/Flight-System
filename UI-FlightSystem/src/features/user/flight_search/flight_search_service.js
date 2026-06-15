import api from "@/configs/axios_config";

export const flightSearchService = {
    searchOutbound: ({ searchParams, classId, filters }) => {
        return api.get("/Flight", {
            params: {
                originCity: searchParams.get("origin") ?? "",
                destinationCity: searchParams.get("destination") ?? "",
                departureDate: searchParams.get("departureDate"),
                classId, pageIndex: 1, pageSize: 10, ...filters 
            }
        });
    },

    searchInbound: ({ searchParams, classId, filters }) => {
        return api.get("/Flight", {
            params: {
                originCity: searchParams.get("destination") ?? "",
                destinationCity: searchParams.get("origin") ?? "",
                departureDate: searchParams.get("returnDate"),
                classId, pageIndex: 1, pageSize: 10, ...filters 
            }
        });
    },

    searchLeg: ({ leg, classId, filters }) => {
        return api.get("/Flight", {
            params: {
                originCity: leg.from,
                destinationCity: leg.to,
                departureDate: leg.departureDate,
                classId, pageIndex: 1, pageSize: 10, ...filters
            }
        });
    }
};