import { useAuth } from '../../contexts/AuthContext';
import React, { useState, useCallback } from 'react';
import type { MouseEvent } from 'react';
import {
    Menu,
    Avatar,
    Button,
    Tooltip,
    MenuItem,
    ListItemIcon,
    ListItemText,
    useTheme,
} from '@mui/material';
import type { Theme } from '@mui/material';
import { Icon } from '@iconify/react';

interface UserMenuItem {
    id: number;
    title: string;
    icon: string;
    action?: () => void;
}

const UserDropDown = () => {
    const { logout } = useAuth();
    const theme = useTheme<Theme>();
    const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
    const menuOpen = Boolean(anchorEl);

    const handleUserClick = useCallback((event: MouseEvent<HTMLElement>) => {
        setAnchorEl(event.currentTarget);
    }, []);

    const handleUserClose = useCallback(() => {
        setAnchorEl(null);
    }, []);

    const handleMenuItemClick = (item: UserMenuItem) => {
        handleUserClose();
        console.log('Tıklanan menü:', item.title);
        if (item.action) item.action();
    };
    const handleLogout = () => {
        logout();
        handleUserClose(); // menüyü kapat
    };
    const userMenuItems: UserMenuItem[] = [
        {
            id: 1,
            title: 'Profil',
            icon: 'mdi:account'
        },
        {
            id: 2,
            title: 'Ayarlar',
            icon: 'mdi:cog'
        },
        {
            id: 3,
            title: 'Çıkış Yap',
            icon: 'mdi:logout',
            action: handleLogout
        }
    ];

    return (
        <>
            <Button
                color="inherit"
                variant="text"
                id="account-dropdown-button"
                aria-controls={menuOpen ? 'account-menu' : undefined}
                aria-haspopup="true"
                aria-expanded={menuOpen ? 'true' : undefined}
                onClick={handleUserClick}
                disableRipple
                sx={{
                    borderRadius: theme.shape.borderRadius,
                    gap: theme.spacing(1),
                    px: { xs: 0.5, sm: 1 },
                    py: 0.5,
                    textTransform: 'none',
                    '&:hover': {
                        bgcolor: theme.palette.action.hover
                    }
                }}
            >
                <Tooltip title="Kullanıcı Adı" arrow placement="bottom">
                    <Avatar sx={{ width: 35, height: 35 }}>
                        <Icon icon="mdi:account" />
                    </Avatar>
                    {/*<Avatar src={profile} sx={{ width: 40, height: 40 }} />*/}
                </Tooltip>

            </Button>

            <Menu
                id="account-menu"
                anchorEl={anchorEl}
                open={menuOpen}
                onClose={handleUserClose}
                transformOrigin={{ horizontal: 'right', vertical: 'top' }}
                anchorOrigin={{ horizontal: 'right', vertical: 'bottom' }}
                slotProps={{
                    paper: {
                        elevation: 3,
                        sx: {
                            mt: 1.5,
                            minWidth: 180
                        }
                    }
                }}
            >
                {userMenuItems.map((item) => (
                    <MenuItem key={item.id} onClick={() => handleMenuItemClick(item)}>
                        <ListItemIcon>
                            <Icon icon={item.icon} width={20} height={20} />
                        </ListItemIcon>
                        <ListItemText>{item.title}</ListItemText>
                    </MenuItem>
                ))}
            </Menu>
        </>
    );
};

export default UserDropDown;
