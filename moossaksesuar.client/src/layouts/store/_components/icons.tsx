export const IconMenu = (props: React.SVGProps<SVGSVGElement>) => (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" className="h-6 w-6" {...props}>
        <path strokeWidth="2" strokeLinecap="round" d="M4 6h16M4 12h16M4 18h16" />
    </svg>
);

export const IconClose = (props: React.SVGProps<SVGSVGElement>) => (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" className="h-6 w-6" {...props}>
        <path strokeWidth="2" strokeLinecap="round" d="M6 6l12 12M18 6l-12 12" />
    </svg>
);

export const IconCart = (props: React.SVGProps<SVGSVGElement>) => (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" className="h-5 w-5" {...props}>
        <circle cx="9" cy="20" r="1"/><circle cx="17" cy="20" r="1"/>
        <path strokeWidth="2" strokeLinecap="round" d="M3 4h2l2.4 12.5a1 1 0 0 0 1 .8h8.6a1 1 0 0 0 1-.8L20 8H6"/>
    </svg>
);

export const IconUser = (props: React.SVGProps<SVGSVGElement>) => (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" className="h-5 w-5" {...props}>
        <path strokeWidth="2" strokeLinecap="round" d="M12 12a5 5 0 1 0-5-5 5 5 0 0 0 5 5Zm0 2c-4 0-8 2-8 6h16c0-4-4-6-8-6Z"/>
    </svg>
);
