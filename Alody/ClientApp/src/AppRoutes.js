import { Counter } from "./components/Counter";
import { FetchData } from "./components/FetchData";
import { Home } from "./components/Home";
import LoginForm from "./components/Login";

const AppRoutes = [
    {
        index: true,
        element: <Home />
    },
    {
        path: '/login',
        element: <LoginForm />
    },
    {
        path: '/counter',
        element: <Counter />
    },
    {
        path: '/home',
        element: <Home />
    },
    {
        path: '/fetch-data',
        element: <FetchData />
    }
];

export default AppRoutes;
