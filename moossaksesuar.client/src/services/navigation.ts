import type { NavigateFunction, NavigateOptions, To } from "react-router-dom";

let navigateRef: NavigateFunction | null = null;

export function setNavigator(nav: NavigateFunction) {
    navigateRef = nav;
}

export function navTo(to: To, options?: NavigateOptions) {
    navigateRef?.(to, options);
}

export function navReplace(to: To, options?: NavigateOptions) {
    navigateRef?.(to, { ...options, replace: true });
}
