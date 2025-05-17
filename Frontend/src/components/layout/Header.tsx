import {useAuth} from '../../contexts/AuthContext';
import React from 'react';
import {AppBar, Toolbar, Typography, IconButton, Badge, Button, Box} from '@mui/material';
import {Icon} from '@iconify/react';
import {Link} from 'react-router-dom';
import UserDropdown from './UserDropDown.tsx';

const Header: React.FC = () => {
    const {isAuthenticated} = useAuth(); // giriş durumu kontrolü
    return (
        <AppBar position="static" color="primary" elevation={3} sx={{ pt: 1.5, pb:1 }}>
            <Toolbar sx={{display: 'flex', justifyContent: 'space-between'}}>
                {/* Sol: Logo */}
                <Box>
                    <Typography component={Link} to="/" variant="h6" color="inherit"
                                sx={{textDecoration: 'none', fontWeight: 'bold'}}>
                        ETicaret
                    </Typography>
                </Box>

                {/* Sağ: Bildirim ve Avatar */}
                <Box sx={{display: 'flex', alignItems: 'center', gap: 2}}>
                    {isAuthenticated ? (
                        <>
                            <Badge
                                color="error"
                                badgeContent=" "
                                variant="dot"
                                sx={{
                                    '& .MuiBadge-badge': {
                                        top: 6,
                                        right: 6,
                                    },
                                }}
                            >
                                <IconButton color="inherit">
                                    <Icon icon="mdi:bell" width={24} height={24}/>
                                </IconButton>
                            </Badge>
                            <UserDropdown/>
                        </>
                    ) : (
                        <Button
                            component={Link}
                            to="/login"
                            variant="outlined"
                            color="inherit"
                            sx={{textTransform: 'none'}}
                        >
                            Giriş / Kayıt
                        </Button>
                    )}
                </Box>

            </Toolbar>
        </AppBar>
    );
};

export default Header;
