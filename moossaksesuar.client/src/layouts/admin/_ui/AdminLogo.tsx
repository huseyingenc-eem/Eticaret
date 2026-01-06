import { Link } from "react-router-dom";
import logoFull from "@/assets/images/logo/logo-dark.png";
import logoCompact from "@/assets/images/logo/logo-sm.png";

type Props = {
    to?: string;
    collapsed?: boolean;
    alt?: string;
};

export default function AdminLogo({ to = "/admin", collapsed = false, alt = "Logo" }: Props) {
    return (
        <Link to={to} aria-label="Go to admin home" className="inline-flex items-center justify-center">
            {collapsed ? (
                <img src={logoCompact} alt={alt} className="w-8 h-8 object-contain" />
            ) : (
                <img src={logoFull} alt={alt} className="h-6 object-contain" />
            )}
        </Link>
    );
}
