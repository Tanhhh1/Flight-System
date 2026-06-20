import { useEffect, useRef } from "react";
import { useDispatch } from "react-redux";
import * as signalR from "@microsoft/signalr";
import { updateSeatStatusFromHub } from "./seat_selection_slice";
import { SEAT_REVERSE_HUB_URL, HUB_EVENTS } from "@/constants/seat_selection";
import { store } from "@/store";

export function useSeatHub(flightId) {
    const dispatch = useDispatch();
    const connectionRef = useRef(null);

    useEffect(() => {
        if (!flightId) return;

        const connection = new signalR.HubConnectionBuilder()
            .withUrl(SEAT_REVERSE_HUB_URL, {
                accessTokenFactory: () => store.getState().auth?.token ?? "",
            })
            .withAutomaticReconnect([0, 2000, 5000, 10000])
            .configureLogging(signalR.LogLevel.Warning)
            .build();

        connectionRef.current = connection;

        connection.on(HUB_EVENTS.SEAT_STATUS_CHANGED, (payload) => {
            dispatch(updateSeatStatusFromHub(payload));
        });

        connection.start()
            .then(() => {
                if (connection.state === signalR.HubConnectionState.Connected) {
                    connection.invoke(HUB_EVENTS.JOIN_FLIGHT, flightId)
                        .catch(err => console.warn("[SeatHub] JoinFlight thất bại:", err));
                }
            })
            .catch((err) => {
                console.warn("[SeatHub] Kết nối thất bại:", err?.message ?? err);
            });

        connection.onreconnected(() => {
            if (connection.state === signalR.HubConnectionState.Connected) {
                connection.invoke(HUB_EVENTS.JOIN_FLIGHT, flightId)
                    .catch(err => console.warn("[SeatHub] Rejoin thất bại:", err));
            }
        });

        return () => {
            connection.stop().catch(() => {});
            connectionRef.current = null;
        };
    }, [flightId, dispatch]);

    return connectionRef;
}