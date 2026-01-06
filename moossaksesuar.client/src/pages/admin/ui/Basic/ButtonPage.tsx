// src/pages/admin/ui/Basic/ButtonPage.tsx
import React, { useMemo, useState } from "react";
import {
    ExternalLink,
    Save,
    Check,
    X,
    Star,
    Upload,
    RefreshCw,
    Download,
    Play,
    Pause,
    SkipForward,
    Volume2,
    Settings,
    Search,
    Filter,
    Copy,
    Share,
    Heart,
    Bookmark,
    MessageCircle,
    User,
    Calendar,
    Mail,
    Phone,
    Camera,
    Image,
    Video,
    Music,
    FileText,
    Folder,
    Lock,
    Eye,
    ChevronDown,
    ChevronUp,
    ChevronLeft,
    ChevronRight,
    Plus,
    Minus,
    Zap,
    Shield,
    Award,
    Target,
    Rocket,
    Sparkles
} from "lucide-react";
import {Button, Spinner, GhostSpinner} from "@/components/ui";
import {
    HeartIcon,
    CreditCardIcon,
    HandThumbUpIcon,
    TrashIcon,
    ArrowsUpDownIcon,
    EllipsisVerticalIcon,
    ShoppingCartIcon,
    BellIcon,
    ChartBarIcon,
    GlobeAltIcon,
    LightBulbIcon,
    FireIcon,
} from "@heroicons/react/24/solid";
import {StarIcon, BeakerIcon} from "@heroicons/react/24/solid";
import {FaPaperPlane, FaGithub, FaTwitter, FaDiscord, FaGoogle, FaApple, FaMicrosoft} from "react-icons/fa";

const maskStyles = `
.mask.is-hexagon {
  mask: polygon(30% 0%, 70% 0%, 100% 50%, 70% 100%, 30% 100%, 0% 50%);
  -webkit-mask: polygon(30% 0%, 70% 0%, 100% 50%, 70% 100%, 30% 100%, 0% 50%);
}

.mask.is-squircle {
  mask: radial-gradient(circle at 50% 50%, black 40%, transparent 40%);
  border-radius: 20% / 50%;
}

.mask.is-triangle {
  mask: polygon(50% 0%, 0% 100%, 100% 100%);
  -webkit-mask: polygon(50% 0%, 0% 100%, 100% 100%);
}

.mask.is-diamond {
  mask: polygon(50% 0%, 100% 50%, 50% 100%, 0% 50%);
  -webkit-mask: polygon(50% 0%, 100% 50%, 50% 100%, 0% 50%);
}

.glass-effect {
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.2);
}

.dark .glass-effect {
  background: rgba(0, 0, 0, 0.1);
  border: 1px solid rgba(255, 255, 255, 0.1);
}

.neumorphism {
  background: #f0f0f0;
  box-shadow: 
    8px 8px 16px #bebebe,
    -8px -8px 16px #ffffff;
}

.dark .neumorphism {
  background: #2d2d2d;
  box-shadow: 
    8px 8px 16px #1a1a1a,
    -8px -8px 16px #404040;
}

.floating {
  animation: float 3s ease-in-out infinite;
}

@keyframes float {
  0%, 100% { transform: translateY(0px); }
  50% { transform: translateY(-5px); }
}

.pulse-ring {
  animation: pulse-ring 1.5s cubic-bezier(0.215, 0.61, 0.355, 1) infinite;
}

@keyframes pulse-ring {
  0% {
    transform: scale(0.8);
    opacity: 1;
  }
  80%, 100% {
    transform: scale(1.2);
    opacity: 0;
  }
}

.magnetic-hover:hover {
  transform: translateY(-2px);
  transition: all 0.3s cubic-bezier(0.34, 1.56, 0.64, 1);
}

.ripple-effect {
  position: relative;
  overflow: hidden;
}

.ripple-effect::before {
  content: '';
  position: absolute;
  top: 50%;
  left: 50%;
  width: 0;
  height: 0;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.5);
  transform: translate(-50%, -50%);
  transition: width 0.6s, height 0.6s;
}

.ripple-effect:active::before {
  width: 300px;
  height: 300px;
}
`;
function Row({children}: { children: React.ReactNode }) {
    return <div className="flex flex-wrap items-start gap-3">{children}</div>;
}

function Section({title, desc, children}: { title: string; desc?: React.ReactNode; children: React.ReactNode }) {
    return (
        <section className="rounded-lg border p-5 transition-all hover:shadow-sm">
            <div className="mb-4 flex items-start justify-between gap-4">
                <div>
                    <h3 className="text-base font-semibold">{title}</h3>
                    {desc && <p className="mt-1 text-sm text-gray-500">{desc}</p>}
                </div>
            </div>
            {children}
        </section>
    );
}

export default function ButtonPage() {
    const colors: Array<Parameters<typeof Button>[0]["color"]> = [
        "neutral",
        "primary",
        "secondary",
        "info",
        "success",
        "warning",
        "danger",
    ];
    const sizeOpts: Array<Parameters<typeof Button>[0]["size"]> = ["xs","sm","md","lg","xl"];
    const variantOpts: Array<Parameters<typeof Button>[0]["variant"]> = ["filled","soft","outline","ghost"];

    const [plLabel, setPlLabel] = useState("Click me");
    const [plColor, setPlColor] = useState<Parameters<typeof Button>[0]["color"]>("primary");
    const [plVariant, setPlVariant] = useState<Parameters<typeof Button>[0]["variant"]>("filled");
    const [plSize, setPlSize] = useState<Parameters<typeof Button>[0]["size"]>("md");
    const [plGlow, setPlGlow] = useState(false);
    const [plIconOnly, setPlIconOnly] = useState(false);
    const [plLoading, setPlLoading] = useState(false);
    const [plDisabled, setPlDisabled] = useState(false);

    const codePreview = useMemo(() => {
        const props: string[] = [];
        if (plColor !== "neutral") props.push(`color="${plColor}"`);
        if (plVariant !== "filled") props.push(`variant="${plVariant}"`);
        if (plSize !== "md") props.push(`size="${plSize}"`);
        if (plGlow) props.push("isGlow");
        if (plIconOnly) props.push("isIcon");
        if (plLoading) props.push("loading");
        if (plDisabled) props.push("disabled");

        const children = plIconOnly
            ? `  <Zap className="h-4 w-4" />`
            : `  <Zap className="mr-2 h-4 w-4" /> ${plLabel || "Button"}`;

        return `<Button ${props.join(" ")}>\n${children}\n</Button>`;
    }, [plColor, plVariant, plSize, plGlow, plIconOnly, plLoading, plDisabled, plLabel]);
    const variants = ["solid", "soft", "outline", "ghost", "flat"];

    // Inject custom styles
    if (typeof document !== 'undefined' && !document.getElementById('mask-styles')) {
        const style = document.createElement('style');
        style.id = 'mask-styles';
        style.textContent = maskStyles;
        document.head.appendChild(style);
    }

    return (
        <div className="space-y-6">

            <Section
                title="Manual Playground"
                desc="Seç, önizle ve yan tarafta JSX kodunu gör."
            >
                <div className="grid gap-4 lg:grid-cols-2">
                    {/* Kontroller + Önizleme */}
                    <div className="space-y-3">
                        <div className="grid grid-cols-2 gap-3">
                            <label className="flex flex-col text-sm">
                                <span className="mb-1 text-gray-600">Label</span>
                                <input
                                    value={plLabel}
                                    onChange={(e) => setPlLabel(e.target.value)}
                                    className="rounded border px-2 py-1"
                                    placeholder="Button text"
                                />
                            </label>

                            <label className="flex flex-col text-sm">
                                <span className="mb-1 text-gray-600">Color</span>
                                <select
                                    value={plColor}
                                    onChange={(e) => setPlColor(e.target.value as any)}
                                    className="rounded border px-2 py-1"
                                >
                                    {colors.map((c) => (
                                        <option key={c} value={c}>{c}</option>
                                    ))}
                                </select>
                            </label>

                            <label className="flex flex-col text-sm">
                                <span className="mb-1 text-gray-600">Variant</span>
                                <select
                                    value={plVariant}
                                    onChange={(e) => setPlVariant(e.target.value as any)}
                                    className="rounded border px-2 py-1"
                                >
                                    {variantOpts.map((v) => (
                                        <option key={v} value={v}>{v}</option>
                                    ))}
                                </select>
                            </label>

                            <label className="flex flex-col text-sm">
                                <span className="mb-1 text-gray-600">Size</span>
                                <select
                                    value={plSize}
                                    onChange={(e) => setPlSize(e.target.value as any)}
                                    className="rounded border px-2 py-1"
                                >
                                    {sizeOpts.map((s) => (
                                        <option key={s} value={s}>{s}</option>
                                    ))}
                                </select>
                            </label>
                        </div>

                        <div className="flex flex-wrap gap-4">
                            <label className="flex items-center gap-2 text-sm">
                                <input type="checkbox" checked={plGlow} onChange={(e) => setPlGlow(e.target.checked)} />
                                Glow
                            </label>
                            <label className="flex items-center gap-2 text-sm">
                                <input type="checkbox" checked={plIconOnly} onChange={(e) => setPlIconOnly(e.target.checked)} />
                                Icon only
                            </label>
                            <label className="flex items-center gap-2 text-sm">
                                <input type="checkbox" checked={plLoading} onChange={(e) => setPlLoading(e.target.checked)} />
                                Loading
                            </label>
                            <label className="flex items-center gap-2 text-sm">
                                <input type="checkbox" checked={plDisabled} onChange={(e) => setPlDisabled(e.target.checked)} />
                                Disabled
                            </label>
                        </div>

                        {/* Önizleme */}
                        <div className="pt-2">
                            <Button
                                color={plColor}
                                variant={plVariant}
                                size={plSize}
                                isGlow={plGlow}
                                isIcon={plIconOnly}
                                loading={plLoading}
                                disabled={plDisabled}
                            >
                                {plIconOnly ? (
                                    <Zap className="h-4 w-4" />
                                ) : (
                                    <>
                                        <Zap className="mr-2 h-4 w-4" />
                                        {plLabel || "Button"}
                                    </>
                                )}
                            </Button>
                        </div>
                    </div>

                    {/* Kod Paneli */}
                    <div className="relative">
                        <button
                            type="button"
                            className="absolute right-2 top-2 rounded border px-2 py-1 text-xs"
                            onClick={() => navigator.clipboard.writeText(codePreview)}
                        >
                            Copy
                        </button>
                        <pre className="h-full min-h-[160px] overflow-auto rounded-lg bg-gray-900 p-4 text-xs text-gray-100">
{codePreview}
      </pre>
                    </div>
                </div>
            </Section>
            {/* Default */}
            <Section title="Default Button" desc={<span>
                A <code className="mx-1 rounded bg-gray-100 px-1">button</code> means an operation. Clicking triggers corresponding logic.
            </span>}>
                <Row>
                    <Button>Default</Button>
                    <Button disabled>Disabled</Button>
                </Row>
            </Section>

            {/* Basic Colors */}
            <Section
                title="Basic Colors"
                desc={<span>
                    <code className="mx-1 rounded bg-gray-100 px-1">Button</code> component supports different colors:
                    <code className="mx-1 rounded bg-gray-100 px-1">neutral</code>,
                    <code className="mx-1 rounded bg-gray-100 px-1">primary</code>,
                    <code className="mx-1 rounded bg-gray-100 px-1">secondary</code>,
                    <code className="mx-1 rounded bg-gray-100 px-1">info</code>,
                    <code className="mx-1 rounded bg-gray-100 px-1">success</code>,
                    <code className="mx-1 rounded bg-gray-100 px-1">warning</code>,
                    <code className="mx-1 rounded bg-gray-100 px-1">danger</code>.
                </span>}
            >
                <Row>
                    {colors.map((c) => (
                        <Button key={c} color={c}>
                            {c === "danger" ? "Danger" : c[0].toUpperCase() + c.slice(1)}
                        </Button>
                    ))}
                </Row>
            </Section>

            {/* Button Sizes */}
            <Section
                title="Button Sizes"
                desc={<span>
                    Different button sizes for various use cases. From compact to extra large.
                </span>}
            >
                <Row>
                    <Button size="xs" color="primary">Extra Small</Button>
                    <Button size="sm" color="primary">Small</Button>
                    <Button size="md" color="primary">Medium</Button>
                    <Button size="lg" color="primary">Large</Button>
                    <Button size="xl" color="primary">Extra Large</Button>
                </Row>
            </Section>

            {/* Variants */}
            <Section
                title="Button Variants"
                desc={<span>
                    Different visual styles: <code className="mx-1 rounded bg-gray-100 px-1">solid</code>,
                    <code className="mx-1 rounded bg-gray-100 px-1">soft</code>,
                    <code className="mx-1 rounded bg-gray-100 px-1">outline</code>,
                    <code className="mx-1 rounded bg-gray-100 px-1">ghost</code>,
                    <code className="mx-1 rounded bg-gray-100 px-1">flat</code>.
                </span>}
            >
                <div className="space-y-4">
                    {variants.map((variant) => (
                        <div key={variant} className="space-y-2">
                            <h4 className="text-sm font-medium text-gray-700 capitalize">{variant}</h4>
                            <Row>
                                {colors.slice(1, 5).map((color) => (
                                    <Button key={`${variant}-${color}`} color={color} variant={variant as any}>
                                        {color[0].toUpperCase() + color.slice(1)}
                                    </Button>
                                ))}
                            </Row>
                        </div>
                    ))}
                </div>
            </Section>

            {/* Rounded Buttons */}
            <Section
                title="Rounded Buttons"
                desc={<span>
                    <code className="mx-1 rounded bg-gray-100 px-1">Button</code> can have different border radius styles.
                </span>}
            >
                <Row>
                    <Button color="primary" className="rounded-none">Square</Button>
                    <Button color="primary" className="rounded-sm">Small Radius</Button>
                    <Button color="primary" className="rounded-md">Medium Radius</Button>
                    <Button color="primary" className="rounded-lg">Large Radius</Button>
                    <Button color="primary" className="rounded-full">Pill</Button>
                </Row>
            </Section>

            {/* Loading States */}
            <Section
                title="Loading States"
                desc={<span>
                    Use <code className="mx-1 rounded bg-gray-100 px-1">loading</code> prop or
                    <code className="mx-1 rounded bg-gray-100 px-1">Spinner</code> components for loading states.
                </span>}
            >
                <Row>
                    <Button color="primary" loading>
                        <Upload className="h-4 w-4"/> Uploading
                    </Button>
                    <Button color="success">
                        <Spinner className="mr-2"/> Processing
                    </Button>
                    <Button color="warning" variant="soft">
                        <GhostSpinner className="mr-2" variant="soft"/> Warming up
                    </Button>
                    <Button color="danger" variant="outline">
                        <GhostSpinner className="mr-2" variant="innerDot"/> Danger op
                    </Button>
                    <Button color="info" disabled loading>
                        Loading...
                    </Button>
                </Row>
            </Section>

            {/* Icon Buttons */}
            <Section
                title="Icon Buttons"
                desc={<span>
                    Square buttons perfect for icons. Use <code className="mx-1 rounded bg-gray-100 px-1">isIcon</code> prop for proper spacing.
                </span>}
            >
                <div className="space-y-4">
                    <div className="space-y-2">
                        <h4 className="text-sm font-medium text-gray-700">Standard Icons</h4>
                        <Row>
                            <Button color="primary" isIcon>
                                <Save className="h-4 w-4"/>
                            </Button>
                            <Button color="success" isIcon>
                                <Check className="h-4 w-4"/>
                            </Button>
                            <Button color="danger" isIcon>
                                <X className="h-4 w-4"/>
                            </Button>
                            <Button color="warning" isIcon variant="soft">
                                <Star className="h-4 w-4"/>
                            </Button>
                            <Button color="info" isIcon variant="outline">
                                <Settings className="h-4 w-4"/>
                            </Button>
                            <Button isIcon variant="ghost">
                                <Search className="h-4 w-4"/>
                            </Button>
                        </Row>
                    </div>

                    <div className="space-y-2">
                        <h4 className="text-sm font-medium text-gray-700">Rounded Icons</h4>
                        <Row>
                            <Button color="secondary" isIcon className="size-9 rounded-full">
                                <HeartIcon className="size-5"/>
                            </Button>
                            <Button color="primary" isIcon className="size-9 rounded-full">
                                <StarIcon className="size-5"/>
                            </Button>
                            <Button color="info" variant="soft" isIcon className="size-9 rounded-full">
                                <CreditCardIcon className="size-5"/>
                            </Button>
                            <Button color="success" isIcon className="size-9 rounded-full">
                                <HandThumbUpIcon className="size-5"/>
                            </Button>
                            <Button color="warning" isIcon className="size-9 rounded-full">
                                <BeakerIcon className="size-5"/>
                            </Button>
                            <Button color="danger" variant="soft" isIcon className="size-9 rounded-full">
                                <TrashIcon className="size-5"/>
                            </Button>
                        </Row>
                    </div>

                    <div className="space-y-2">
                        <h4 className="text-sm font-medium text-gray-700">Special Shapes</h4>
                        <Row>
                            <Button color="primary" isIcon className="mask is-hexagon size-9">
                                <ArrowsUpDownIcon className="size-5"/>
                            </Button>
                            <Button color="info" isIcon className="mask is-squircle size-9">
                                <FaPaperPlane className="size-4"/>
                            </Button>
                            <Button color="success" isIcon className="mask is-triangle size-9">
                                <Play className="size-4"/>
                            </Button>
                            <Button color="warning" isIcon className="mask is-diamond size-9">
                                <Zap className="size-4"/>
                            </Button>
                        </Row>
                    </div>
                </div>
            </Section>

            {/* Button Groups */}
            <Section
                title="Button Groups"
                desc={<span>
                    Combine related buttons together for better UX.
                </span>}
            >
                <div className="space-y-4">
                    <div className="space-y-2">
                        <h4 className="text-sm font-medium text-gray-700">Media Controls</h4>
                        <div className="flex rounded-lg border p-1">
                            <Button variant="ghost" size="sm" isIcon>
                                <SkipForward className="h-4 w-4 rotate-180"/>
                            </Button>
                            <Button variant="ghost" size="sm" isIcon>
                                <Play className="h-4 w-4"/>
                            </Button>
                            <Button variant="ghost" size="sm" isIcon>
                                <Pause className="h-4 w-4"/>
                            </Button>
                            <Button variant="ghost" size="sm" isIcon>
                                <SkipForward className="h-4 w-4"/>
                            </Button>
                            <div className="mx-2 border-l"></div>
                            <Button variant="ghost" size="sm" isIcon>
                                <Volume2 className="h-4 w-4"/>
                            </Button>
                        </div>
                    </div>

                    <div className="space-y-2">
                        <h4 className="text-sm font-medium text-gray-700">Text Formatting</h4>
                        <div className="flex rounded-lg border p-1">
                            <Button variant="ghost" size="sm">Bold</Button>
                            <Button variant="ghost" size="sm">Italic</Button>
                            <Button variant="ghost" size="sm">Underline</Button>
                            <div className="mx-2 border-l"></div>
                            <Button variant="ghost" size="sm" isIcon>
                                <Copy className="h-4 w-4"/>
                            </Button>
                            <Button variant="ghost" size="sm" isIcon>
                                <Share className="h-4 w-4"/>
                            </Button>
                        </div>
                    </div>

                    <div className="space-y-2">
                        <h4 className="text-sm font-medium text-gray-700">Navigation</h4>
                        <div className="flex rounded-lg border p-1">
                            <Button variant="ghost" size="sm" isIcon>
                                <ChevronLeft className="h-4 w-4"/>
                            </Button>
                            <Button variant="ghost" size="sm">1</Button>
                            <Button color="primary" size="sm">2</Button>
                            <Button variant="ghost" size="sm">3</Button>
                            <Button variant="ghost" size="sm">...</Button>
                            <Button variant="ghost" size="sm">10</Button>
                            <Button variant="ghost" size="sm" isIcon>
                                <ChevronRight className="h-4 w-4"/>
                            </Button>
                        </div>
                    </div>
                </div>
            </Section>

            {/* Social Buttons */}
            <Section
                title="Social & Brand Buttons"
                desc={<span>
                    Ready-to-use social media and brand integration buttons.
                </span>}
            >
                <div className="space-y-4">
                    <div className="space-y-2">
                        <h4 className="text-sm font-medium text-gray-700">Social Login</h4>
                        <Row>
                            <Button variant="outline" className="border-gray-300">
                                <FaGoogle className="mr-2 h-4 w-4"/>
                                Continue with Google
                            </Button>
                            <Button className="bg-black text-white hover:bg-gray-800">
                                <FaApple className="mr-2 h-4 w-4"/>
                                Sign in with Apple
                            </Button>
                            <Button className="bg-blue-600 text-white hover:bg-blue-700">
                                <FaMicrosoft className="mr-2 h-4 w-4"/>
                                Microsoft
                            </Button>
                        </Row>
                    </div>

                    <div className="space-y-2">
                        <h4 className="text-sm font-medium text-gray-700">Social Share</h4>
                        <Row>
                            <Button className="bg-gray-900 text-white hover:bg-gray-800">
                                <FaGithub className="mr-2 h-4 w-4"/>
                                GitHub
                            </Button>
                            <Button className="bg-blue-500 text-white hover:bg-blue-600">
                                <FaTwitter className="mr-2 h-4 w-4"/>
                                Twitter
                            </Button>
                            <Button className="bg-indigo-600 text-white hover:bg-indigo-700">
                                <FaDiscord className="mr-2 h-4 w-4"/>
                                Discord
                            </Button>
                        </Row>
                    </div>
                </div>
            </Section>

            {/* Glow Effects */}
            <Section
                title="Glow Effects"
                desc={<span>
                    Add <code className="mx-1 rounded bg-gray-100 px-1">isGlow</code> to enable beautiful glow effects.
                </span>}
            >
                <Row>
                    <Button isGlow>Neutral Glow</Button>
                    <Button isGlow color="primary">Primary Glow</Button>
                    <Button isGlow color="secondary">Secondary Glow</Button>
                    <Button isGlow color="info">Info Glow</Button>
                    <Button isGlow color="success">Success Glow</Button>
                    <Button isGlow color="warning">Warning Glow</Button>
                    <Button isGlow color="danger">Danger Glow</Button>
                </Row>
            </Section>

            {/* Gradient Buttons */}
            <Section
                title="Gradient Buttons"
                desc={<span>
                    Beautiful gradient buttons using <code className="mx-1 rounded bg-gray-100 px-1">unstyled</code> prop for custom styling.
                </span>}
            >
                <div className="space-y-4">
                    <div className="space-y-2">
                        <h4 className="text-sm font-medium text-gray-700">Solid Gradients</h4>
                        <Row>
                            <Button
                                unstyled
                                className="rounded-lg bg-gradient-to-r from-fuchsia-600 to-pink-600 px-5 py-2 text-white duration-100 ease-out [contain:paint] hover:opacity-85 focus:opacity-85 active:translate-y-px magnetic-hover"
                            >
                                Pink Gradient
                            </Button>
                            <Button
                                unstyled
                                className="rounded-lg bg-gradient-to-br from-purple-500 to-indigo-600 px-5 py-2 text-white duration-100 ease-out [contain:paint] hover:opacity-85 focus:opacity-85 active:translate-y-px magnetic-hover"
                            >
                                Purple Gradient
                            </Button>
                            <Button
                                unstyled
                                className="rounded-lg bg-gradient-to-r from-sky-400 to-blue-600 px-5 py-2 text-white duration-100 ease-out [contain:paint] hover:opacity-85 focus:opacity-85 active:translate-y-px magnetic-hover"
                            >
                                Blue Gradient
                            </Button>
                            <Button
                                unstyled
                                className="rounded-lg bg-gradient-to-r from-amber-400 to-orange-600 px-5 py-2 text-white duration-100 ease-out [contain:paint] hover:opacity-85 focus:opacity-85 active:translate-y-px magnetic-hover"
                            >
                                Orange Gradient
                            </Button>
                            <Button
                                unstyled
                                className="rounded-lg bg-gradient-to-r from-green-400 to-emerald-600 px-5 py-2 text-white duration-100 ease-out [contain:paint] hover:opacity-85 focus:opacity-85 active:translate-y-px magnetic-hover"
                            >
                                Green Gradient
                            </Button>
                        </Row>
                    </div>

                    <div className="space-y-2">
                        <h4 className="text-sm font-medium text-gray-700">Outlined Gradients</h4>
                        <Row>
                            <Button
                                unstyled
                                className="group h-10 rounded-lg bg-gradient-to-r from-amber-400 to-orange-600 p-0.5 duration-100 ease-out [contain:paint] active:translate-y-px magnetic-hover"
                            >
                                <span
                                    className="inline-flex h-full items-center justify-center rounded-lg bg-white px-5 transition-colors hover:text-white group-hover:bg-transparent group-hover:text-white group-focus:bg-transparent group-focus:text-white dark:bg-gray-800">
                                    Orange Outline
                                </span>
                            </Button>

                            <Button
                                unstyled
                                className="group h-10 rounded-lg bg-gradient-to-r from-sky-400 to-blue-600 p-0.5 duration-100 ease-out [contain:paint] active:translate-y-px magnetic-hover"
                            >
                                <span
                                    className="inline-flex h-full items-center justify-center rounded-lg bg-white px-5 transition-colors hover:text-white group-hover:bg-transparent group-hover:text-white group-focus:bg-transparent group-focus:text-white dark:bg-gray-800">
                                    Blue Outline
                                </span>
                            </Button>

                            <Button
                                unstyled
                                className="group h-10 rounded-lg bg-gradient-to-l from-pink-400 to-purple-500 p-0.5 duration-100 ease-out [contain:paint] active:translate-y-px magnetic-hover"
                            >
                                <span
                                    className="inline-flex h-full items-center justify-center rounded-lg bg-white px-5 transition-colors hover:text-white group-hover:bg-transparent group-hover:text-white group-focus:bg-transparent group-focus:text-white dark:bg-gray-800">
                                    Pink Outline
                                </span>
                            </Button>

                            <Button
                                unstyled
                                className="group h-10 rounded-lg bg-gradient-to-r from-green-400 to-blue-500 p-0.5 duration-100 ease-out [contain:paint] active:translate-y-px magnetic-hover"
                            >
                                <span
                                    className="inline-flex h-full items-center justify-center rounded-lg bg-white px-5 transition-colors hover:text-white group-hover:bg-transparent group-hover:text-white group-focus:bg-transparent group-focus:text-white dark:bg-gray-800">
                                    Green Outline
                                </span>
                            </Button>
                        </Row>
                    </div>
                </div>
            </Section>

            {/* Modern Effects */}
            <Section
                title="Modern Effects"
                desc={<span>
                    Advanced visual effects including glassmorphism, neumorphism, and animations.
                </span>}
            >
                <div className="space-y-4">
                    <div className="space-y-2">
                        <h4 className="text-sm font-medium text-gray-700">Glassmorphism</h4>
                        <Row>
                            <Button
                                unstyled
                                className="glass-effect px-4 py-2 text-gray-800 transition-all hover:bg-white/20 dark:text-white magnetic-hover"
                            >
                                Glass Effect
                            </Button>
                            <Button
                                unstyled
                                className="glass-effect rounded-full px-6 py-2 text-gray-800 transition-all hover:bg-white/20 dark:text-white magnetic-hover"
                            >
                                Glass Pill
                            </Button>
                        </Row>
                    </div>

                    <div className="space-y-2">
                        <h4 className="text-sm font-medium text-gray-700">Neumorphism</h4>
                        <Row>
                            <Button
                                unstyled
                                className="neumorphism px-4 py-2 text-gray-800 transition-all hover:shadow-inner dark:text-white"
                            >
                                Neumorphic
                            </Button>
                            <Button
                                unstyled
                                className="neumorphism rounded-full px-6 py-2 text-gray-800 transition-all hover:shadow-inner dark:text-white"
                            >
                                Neu Pill
                            </Button>
                        </Row>
                    </div>

                    <div className="space-y-2">
                        <h4 className="text-sm font-medium text-gray-700">Animated Effects</h4>
                        <Row>
                            <Button
                                unstyled
                                className="floating rounded-lg bg-gradient-to-r from-purple-500 to-pink-500 px-4 py-2 text-white shadow-lg"
                            >
                                <Sparkles className="mr-2 h-4 w-4"/>
                                Floating
                            </Button>
                            <Button
                                unstyled
                                className="ripple-effect rounded-lg bg-blue-600 px-4 py-2 text-white transition-all hover:bg-blue-700"
                            >
                                Ripple Effect
                            </Button>
                            <Button
                                color="success"
                                className="relative overflow-hidden"
                            >
                                <span className="absolute inset-0 pulse-ring rounded-lg bg-green-400 opacity-30"></span>
                                <span className="relative">
                                    <Target className="mr-2 h-4 w-4"/>
                                    Pulsing
                                </span>
                            </Button>
                        </Row>
                    </div>
                </div>
            </Section>

            {/* Call-to-Action Buttons */}
            <Section
                title="Call-to-Action Buttons"
                desc={<span>
                    High-impact buttons designed to drive user action and engagement.
                </span>}
            >
                <div className="space-y-4">
                    <Row>
                        <Button
                            size="lg"
                            color="primary"
                            className="shadow-lg shadow-blue-500/25 transition-all hover:shadow-xl hover:shadow-blue-500/40 magnetic-hover"
                        >
                            <Rocket className="mr-2 h-5 w-5"/>
                            Get Started
                        </Button>

                        <Button
                            size="lg"
                            color="success"
                            isGlow
                            className="magnetic-hover"
                        >
                            <Shield className="mr-2 h-5 w-5"/>
                            Start Free Trial
                        </Button>

                        <Button
                            size="lg"
                            unstyled
                            className="rounded-lg bg-gradient-to-r from-orange-500 to-red-600 px-6 py-3 text-white shadow-lg shadow-orange-500/25 transition-all hover:shadow-xl hover:shadow-orange-500/40 magnetic-hover"
                        >
                            <Award className="mr-2 h-5 w-5"/>
                            Upgrade to Pro
                        </Button>
                    </Row>
                </div>
            </Section>

            {/* Accessibility Features */}
            <Section
                title="Accessibility Features"
                desc={<span>
                    Buttons with proper ARIA labels, focus states, and keyboard navigation support.
                </span>}
            >
                <div className="space-y-4">
                    <div className="space-y-2">
                        <h4 className="text-sm font-medium text-gray-700">Focus States</h4>
                        <Row>
                            <Button color="primary" className="focus:ring-2 focus:ring-blue-500 focus:ring-offset-2">
                                Focus Ring
                            </Button>
                            <Button color="success" className="focus:ring-2 focus:ring-green-500 focus:ring-offset-2">
                                Custom Focus
                            </Button>
                            <Button color="warning" className="focus:outline-none focus:ring-2 focus:ring-yellow-500">
                                No Outline
                            </Button>
                        </Row>
                    </div>

                    <div className="space-y-2">
                        <h4 className="text-sm font-medium text-gray-700">Screen Reader Support</h4>
                        <Row>
                            <Button isIcon color="primary" aria-label="Save document">
                                <Save className="h-4 w-4"/>
                            </Button>
                            <Button isIcon color="danger" aria-label="Delete item" title="Delete item">
                                <TrashIcon className="h-4 w-4"/>
                            </Button>
                            <Button aria-describedby="help-text">
                                Need Help?
                            </Button>
                            <span id="help-text" className="sr-only">
                                This button opens the help documentation
                            </span>
                        </Row>
                    </div>
                </div>
            </Section>

            {/* Interactive States */}
            <Section
                title="Interactive States"
                desc={<span>
                    Buttons demonstrating various interactive states and behaviors.
                </span>}
            >
                <div className="space-y-4">
                    <div className="space-y-2">
                        <h4 className="text-sm font-medium text-gray-700">Toggle States</h4>
                        <Row>
                            <Button color="primary" className="data-[state=on]:bg-blue-600 data-[state=on]:text-white">
                                <Eye className="mr-2 h-4 w-4"/>
                                Show
                            </Button>
                            <Button variant="outline" className="data-[pressed=true]:bg-gray-100">
                                <Lock className="mr-2 h-4 w-4"/>
                                Locked
                            </Button>
                            <Button color="success">
                                <Heart className="mr-2 h-4 w-4 fill-current"/>
                                Liked (42)
                            </Button>
                        </Row>
                    </div>

                    <div className="space-y-2">
                        <h4 className="text-sm font-medium text-gray-700">Counter Buttons</h4>
                        <Row>
                            <div className="flex items-center space-x-2">
                                <Button size="sm" isIcon variant="outline">
                                    <Minus className="h-4 w-4"/>
                                </Button>
                                <span className="min-w-8 text-center font-mono">0</span>
                                <Button size="sm" isIcon variant="outline">
                                    <Plus className="h-4 w-4"/>
                                </Button>
                            </div>
                            <div className="flex items-center rounded-lg border">
                                <Button variant="ghost" size="sm" className="rounded-r-none">
                                    <ChevronDown className="h-4 w-4"/>
                                </Button>
                                <div className="border-l border-r px-3 py-2 text-sm">5</div>
                                <Button variant="ghost" size="sm" className="rounded-l-none">
                                    <ChevronUp className="h-4 w-4"/>
                                </Button>
                            </div>
                        </Row>
                    </div>
                </div>
            </Section>

            {/* E-commerce Buttons */}
            <Section
                title="E-commerce & Actions"
                desc={<span>
                    Common e-commerce and action buttons with appropriate styling and icons.
                </span>}
            >
                <div className="space-y-4">
                    <div className="space-y-2">
                        <h4 className="text-sm font-medium text-gray-700">Shopping Actions</h4>
                        <Row>
                            <Button color="primary" size="lg">
                                <ShoppingCartIcon className="mr-2 h-5 w-5"/>
                                Add to Cart
                            </Button>
                            <Button
                                color="success"
                                size="lg"
                                className="shadow-lg shadow-green-500/25"
                            >
                                <CreditCardIcon className="mr-2 h-5 w-5"/>
                                Buy Now
                            </Button>
                            <Button variant="outline">
                                <Bookmark className="mr-2 h-4 w-4"/>
                                Save for Later
                            </Button>
                            <Button variant="ghost">
                                <Share className="mr-2 h-4 w-4"/>
                                Share
                            </Button>
                        </Row>
                    </div>

                    <div className="space-y-2">
                        <h4 className="text-sm font-medium text-gray-700">File Actions</h4>
                        <Row>
                            <Button color="info">
                                <Download className="mr-2 h-4 w-4"/>
                                Download
                            </Button>
                            <Button variant="outline">
                                <Upload className="mr-2 h-4 w-4"/>
                                Upload
                            </Button>
                            <Button color="secondary">
                                <Copy className="mr-2 h-4 w-4"/>
                                Copy Link
                            </Button>
                            <Button variant="ghost" color="danger">
                                <TrashIcon className="mr-2 h-4 w-4"/>
                                Delete
                            </Button>
                        </Row>
                    </div>

                    <div className="space-y-2">
                        <h4 className="text-sm font-medium text-gray-700">Communication</h4>
                        <Row>
                            <Button color="primary">
                                <Mail className="mr-2 h-4 w-4"/>
                                Send Email
                            </Button>
                            <Button color="success">
                                <Phone className="mr-2 h-4 w-4"/>
                                Call Now
                            </Button>
                            <Button color="info">
                                <MessageCircle className="mr-2 h-4 w-4"/>
                                Live Chat
                            </Button>
                            <Button variant="outline">
                                <Calendar className="mr-2 h-4 w-4"/>
                                Schedule
                            </Button>
                        </Row>
                    </div>
                </div>
            </Section>

            {/* Polymorphic Examples */}
            <Section
                title="Polymorphic Components"
                desc={<span>
                    <code className="mx-1 rounded bg-gray-100 px-1">Button</code> is polymorphic.
                    Change the root element via <code className="mx-1 rounded bg-gray-100 px-1">component</code> prop.
                </span>}
            >
                <Row>
                    <Button component="a" href="#" color="primary">
                        <ExternalLink className="mr-2 h-4 w-4"/>
                        External Link
                    </Button>
                    <Button component="a" href="#" variant="outline" target="_blank">
                        <GlobeAltIcon className="mr-2 h-4 w-4"/>
                        Visit Website
                    </Button>
                    <Button component="a" href="mailto:hello@example.com" color="info">
                        <Mail className="mr-2 h-4 w-4"/>
                        Email Us
                    </Button>
                </Row>
            </Section>

            {/* Responsive Buttons */}
            <Section
                title="Responsive Design"
                desc={<span>
                    Buttons that adapt to different screen sizes using responsive utilities.
                </span>}
            >
                <Row>
                    <Button
                        color="primary"
                        className="w-full sm:w-auto"
                        size="sm sm:md md:lg"
                    >
                        <User className="mr-2 h-4 w-4 sm:h-4 sm:w-4 md:h-5 md:w-5"/>
                        <span className="hidden sm:inline">Responsive</span>
                        <span className="sm:hidden">Resp</span>
                    </Button>
                    <Button
                        variant="outline"
                        className="px-2 sm:px-4 md:px-6"
                    >
                        Adaptive Padding
                    </Button>
                </Row>
            </Section>

            {/* Gaming & Entertainment */}
            <Section
                title="Gaming & Entertainment"
                desc={<span>
                    Specialized buttons for gaming interfaces and entertainment apps.
                </span>}
            >
                <div className="space-y-4">
                    <div className="space-y-2">
                        <h4 className="text-sm font-medium text-gray-700">Media Controls</h4>
                        <Row>
                            <Button
                                color="primary"
                                isIcon
                                size="lg"
                                className="rounded-full shadow-lg"
                            >
                                <Play className="h-6 w-6"/>
                            </Button>
                            <Button isIcon size="lg" variant="outline" className="rounded-full">
                                <Pause className="h-6 w-6"/>
                            </Button>
                            <Button isIcon variant="ghost">
                                <SkipForward className="h-5 w-5 rotate-180"/>
                            </Button>
                            <Button isIcon variant="ghost">
                                <SkipForward className="h-5 w-5"/>
                            </Button>
                            <Button isIcon variant="outline" className="rounded-full">
                                <Volume2 className="h-5 w-5"/>
                            </Button>
                        </Row>
                    </div>

                    <div className="space-y-2">
                        <h4 className="text-sm font-medium text-gray-700">Gaming Actions</h4>
                        <Row>
                            <Button
                                unstyled
                                className="rounded-lg bg-gradient-to-r from-yellow-400 to-orange-500 px-4 py-2 font-bold text-white shadow-lg hover:from-yellow-500 hover:to-orange-600 magnetic-hover"
                            >
                                <Zap className="mr-2 h-4 w-4"/>
                                POWER UP!
                            </Button>
                            <Button
                                color="danger"
                                className="font-bold uppercase tracking-wider"
                            >
                                <FireIcon className="mr-2 h-4 w-4"/>
                                Battle
                            </Button>
                            <Button
                                color="success"
                                variant="soft"
                                className="font-semibold"
                            >
                                <Award className="mr-2 h-4 w-4"/>
                                Collect Reward
                            </Button>
                        </Row>
                    </div>
                </div>
            </Section>

            {/* Data & Analytics */}
            <Section
                title="Data & Analytics"
                desc={<span>
                    Buttons specifically designed for dashboards and data-driven interfaces.
                </span>}
            >
                <Row>
                    <Button variant="outline" size="sm">
                        <Filter className="mr-2 h-4 w-4"/>
                        Filter
                    </Button>
                    <Button variant="outline" size="sm">
                        <ChartBarIcon className="mr-2 h-4 w-4"/>
                        Export
                    </Button>
                    <Button color="info" size="sm">
                        <RefreshCw className="mr-2 h-4 w-4"/>
                        Refresh Data
                    </Button>
                    <Button variant="ghost" size="sm" isIcon>
                        <Settings className="h-4 w-4"/>
                    </Button>
                    <Button variant="ghost" size="sm" isIcon>
                        <EllipsisVerticalIcon className="h-4 w-4"/>
                    </Button>
                </Row>
            </Section>

            {/* Creative & Design */}
            <Section
                title="Creative & Design Tools"
                desc={<span>
                    Buttons for creative applications and design tools.
                </span>}
            >
                <div className="space-y-4">
                    <div className="space-y-2">
                        <h4 className="text-sm font-medium text-gray-700">Media Tools</h4>
                        <Row>
                            <Button color="primary" variant="soft">
                                <Camera className="mr-2 h-4 w-4"/>
                                Camera
                            </Button>
                            <Button color="secondary" variant="soft">
                                <Image className="mr-2 h-4 w-4"/>
                                Gallery
                            </Button>
                            <Button color="info" variant="soft">
                                <Video className="mr-2 h-4 w-4"/>
                                Video
                            </Button>
                            <Button color="success" variant="soft">
                                <Music className="mr-2 h-4 w-4"/>
                                Audio
                            </Button>
                        </Row>
                    </div>

                    <div className="space-y-2">
                        <h4 className="text-sm font-medium text-gray-700">File Management</h4>
                        <Row>
                            <Button variant="outline">
                                <Folder className="mr-2 h-4 w-4"/>
                                New Folder
                            </Button>
                            <Button variant="outline">
                                <FileText className="mr-2 h-4 w-4"/>
                                New Document
                            </Button>
                            <Button color="primary">
                                <Upload className="mr-2 h-4 w-4"/>
                                Upload Files
                            </Button>
                        </Row>
                    </div>
                </div>
            </Section>

            {/* Status Indicators */}
            <Section
                title="Status Indicators"
                desc={<span>
                    Buttons that communicate system status and states.
                </span>}
            >
                <div className="space-y-4">
                    <div className="space-y-2">
                        <h4 className="text-sm font-medium text-gray-700">System Status</h4>
                        <Row>
                            <Button color="success" className="relative">
                                <div
                                    className="absolute -top-1 -right-1 h-3 w-3 rounded-full bg-green-500 animate-pulse"></div>
                                <div className="mr-2 h-2 w-2 rounded-full bg-green-400"></div>
                                Online
                            </Button>
                            <Button color="warning" className="relative">
                                <div className="mr-2 h-2 w-2 rounded-full bg-yellow-400 animate-pulse"></div>
                                Maintenance
                            </Button>
                            <Button color="danger" className="relative">
                                <div className="mr-2 h-2 w-2 rounded-full bg-red-400"></div>
                                Offline
                            </Button>
                        </Row>
                    </div>

                    <div className="space-y-2">
                        <h4 className="text-sm font-medium text-gray-700">Notifications</h4>
                        <Row>
                            <Button variant="outline" className="relative">
                                <BellIcon className="mr-2 h-4 w-4"/>
                                <span
                                    className="absolute -top-2 -right-2 flex h-5 w-5 items-center justify-center rounded-full bg-red-500 text-xs text-white">
                                    3
                                </span>
                                Alerts
                            </Button>
                            <Button color="info" className="relative">
                                <MessageCircle className="mr-2 h-4 w-4"/>
                                <span className="absolute -top-1 -right-1 h-2 w-2 rounded-full bg-blue-500"></span>
                                Messages
                            </Button>
                        </Row>
                    </div>
                </div>
            </Section>

            {/* Custom Animations */}
            <Section
                title="Advanced Animations"
                desc={<span>
                    Buttons with complex animations and micro-interactions.
                </span>}
            >
                <Row>
                    <Button
                        unstyled
                        className="group relative overflow-hidden rounded-lg bg-gradient-to-r from-blue-600 to-purple-600 px-6 py-3 text-white transition-all duration-300 hover:scale-105 hover:shadow-xl"
                    >
                        <span
                            className="absolute inset-0 bg-gradient-to-r from-purple-600 to-blue-600 opacity-0 transition-opacity duration-300 group-hover:opacity-100"></span>
                        <span className="relative flex items-center">
                            <Sparkles className="mr-2 h-4 w-4 animate-spin group-hover:animate-pulse"/>
                            Magical
                        </span>
                    </Button>

                    <Button
                        unstyled
                        className="group relative overflow-hidden rounded-lg bg-gray-900 px-6 py-3 text-white transition-all duration-500 hover:bg-gray-800"
                    >
                        <span
                            className="absolute left-0 top-0 h-full w-0 bg-gradient-to-r from-cyan-500 to-blue-500 transition-all duration-500 group-hover:w-full"></span>
                        <span className="relative flex items-center">
                            <Rocket className="mr-2 h-4 w-4 transition-transform duration-500 group-hover:rotate-45"/>
                            Slide Effect
                        </span>
                    </Button>

                    <Button
                        color="primary"
                        className="group relative transition-all duration-300 hover:scale-110"
                    >
                        <span
                            className="absolute inset-0 rounded-lg bg-white opacity-0 transition-opacity duration-300 group-hover:opacity-20"></span>
                        <span className="relative flex items-center">
                            <LightBulbIcon
                                className="mr-2 h-4 w-4 transition-all duration-300 group-hover:animate-bounce"/>
                            Bounce
                        </span>
                    </Button>
                </Row>
            </Section>
        </div>
    );
}