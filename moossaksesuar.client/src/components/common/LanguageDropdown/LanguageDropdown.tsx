import React from 'react';
import {Dropdown} from '@/components/ui';

const LANGS = {
    tr: { label: 'Türkçe', flag: 'https://flagcdn.com/24x18/tr.png' },
    en: { label: 'English', flag: 'https://flagcdn.com/24x18/gb.png' },
};

const LanguageDropdown: React.FC = () => {
    const [lang, setLang] = React.useState<string>(() => localStorage.getItem('LANG') || 'tr');

    const change = (k: string) => {
        setLang(k);
        localStorage.setItem('LANG', k);
        // i18n kullanıyorsan: i18n.changeLanguage(k);
    };

    return (
        <Dropdown className="relative flex items-center h-header">
            <Dropdown.Trigger
                as="button"
                id="flagsDropdown"
                className="inline-flex justify-center items-center p-0 size-[37.5px] rounded-md bg-topbar text-topbar-item hover:bg-topbar-item-bg-hover"
            >
                <img src={LANGS[lang as 'tr'|'en'].flag} alt="lang" className="h-5 rounded-sm" />
            </Dropdown.Trigger>
            <Dropdown.Content
                placement="right-end"
                className="!top-4 min-w-[10rem] p-4 flex flex-col gap-3 bg-white"
            >
                {Object.keys(LANGS).map((k) => (
                    <button key={k} onClick={() => change(k)} className="flex items-center gap-3 hover:text-custom-500">
                        <img src={LANGS[k as 'tr'|'en'].flag} className="h-4 rounded-full" />
                        <span>{LANGS[k as 'tr'|'en'].label}</span>
                    </button>
                ))}
            </Dropdown.Content>
        </Dropdown>
    );
};

export default LanguageDropdown;
