import { createBrowserRouter } from "react-router-dom";
import LoginPage from "../features/auth/LoginPage";
import CreateRoomingHousePage from "../features/rooming-houses/CreateRoomingHousePage";
import LandlordHousePage from "../features/rooming-houses/LandlordHousePage";
import CreateRoomPage from "../features/rooms/CreateRoomPage";
import HomePage from "../pages/HomePage";

export const router = createBrowserRouter([
  {
    path: "/",
    element: <HomePage />,
  },
  {
    path: "/login",
    element: <LoginPage />,
  },
  {
    path: "/landlord/rooming-houses",
    element: <LandlordHousePage />,
  },
  {
    path: "/landlord/rooming-houses/create",
    element: <CreateRoomingHousePage />,
  },
  {
    path: "/landlord/rooming-houses/:roomingHouseId/rooms/create",
    element: <CreateRoomPage />,
  },
]);
