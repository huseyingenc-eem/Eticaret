import React from 'react';
import { Box, Container, Typography } from '@mui/material';

const Footer: React.FC = () => {
    return (
        <Box
            component="footer"
            sx={{
                backgroundColor: 'primary.dark',
                color: 'white',
                py: 2,
                mt: 'auto',
                textAlign: 'center',
            }}
        >
            <Container maxWidth="lg">
                <Typography variant="body2">
                    &copy; {new Date().getFullYear()} E-Ticaret Projesi | Tüm Hakları Saklıdır.
                </Typography>
            </Container>
        </Box>
    );
};

export default Footer;
