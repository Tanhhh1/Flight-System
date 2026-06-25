import { sharedSlice } from "@/store/shared_slice";
import { supportRequestService } from "./support_service";

const { slice, fetchItems: fetchMySupportRequests } = sharedSlice(
    "mySupportRequests",
    supportRequestService.getMy
);

export { fetchMySupportRequests };
export const { setPage } = slice.actions;
export default slice.reducer;

// sửa lỗi approve / reject support request
// lỗi thanh toán bằng thẻ quốc tế, qr code