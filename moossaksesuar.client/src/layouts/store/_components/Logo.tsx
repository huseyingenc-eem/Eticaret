import clsx from "clsx";

type Props = { className?: string };
export default function Logo({ className }: Props) {
    return (
        <span className={clsx("inline-flex items-center gap-2 font-bold tracking-tight", className)}>
            <span>MAAS</span>
    </span>
    );
}
