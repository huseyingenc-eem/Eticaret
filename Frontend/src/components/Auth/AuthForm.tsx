import { useAuth } from '../../contexts/AuthContext';
import { Navigate } from 'react-router-dom';
import React, {useState,useEffect } from 'react';
import {
    Box,
    Button,
    Card,
    Grid,
    TextField,
    Typography,
    Alert,
    useTheme
} from '@mui/material';
import {loginUser, registerUser} from '../../services/AuthService/authService.ts';
import {useNavigate} from 'react-router-dom';



const AuthForm: React.FC = () => {
    const { login } = useAuth()
    const [isRegistering, setIsRegistering] = useState(false);
    const [loginEmail, setLoginEmail] = useState('');
    const [loginPassword, setLoginPassword] = useState('');
    const [registerFirstName, setRegisterFirstName] = useState('');
    const [registerLastName, setRegisterLastName] = useState('');
    const [registerEmail, setRegisterEmail] = useState('');
    const [registerPassword, setRegisterPassword] = useState('');
    const [error, setError] = useState<string | null>(null);
    const navigate = useNavigate();
    const theme = useTheme();

    const handleLoginSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError(null);
        try {
            const result = await loginUser({ email: loginEmail, password: loginPassword });
            login(result.token); // ✅ Context üzerinden login yap
            navigate('/'); // Ana sayfaya yönlendir
        } catch (err: any) {
            setError(err.userFriendlyMessage || err.message || 'Login failed');
        }
    };

    const handleRegisterSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError(null);
        try {
            const result = await registerUser({
                firstName: registerFirstName,
                lastName: registerLastName,
                email: registerEmail,
                password: registerPassword,
                userName: `${registerFirstName}_${registerLastName}` // <- userName hatası için eklendi
            });
            alert('Kayıt başarılı! Giriş yapabilirsiniz.');
            setIsRegistering(false);
        } catch (err: any) {
            setError(err.userFriendlyMessage || err.message || 'Registration failed');
        }
    };
    if (useAuth().isAuthenticated) {
        return <Navigate to="/" replace />;
    }
    return (
        <Box
            display="flex"
            alignItems="center"
            justifyContent="center"
            minHeight="100vh"
            bgcolor={theme.palette.background.default}
            px={2}
        >
            <Card sx={{maxWidth: 900, width: '100%', borderRadius: 4, overflow: 'hidden'}}>
                <Grid container>
                    {/* Sol panel */}
                    <Grid
                        item
                        xs={12}
                        md={5}
                        sx={{
                            bgcolor: 'primary.main',
                            color: 'white',
                            p: 4,
                            display: 'flex',
                            flexDirection: 'column',
                            justifyContent: 'center',
                            textAlign: 'center'
                        }}
                    >
                        <Typography variant="h5" fontWeight="bold" mb={1}>
                            {isRegistering ? 'Welcome Back!' : 'Hello, Welcome!'}
                        </Typography>
                        <Typography variant="body2" mb={3}>
                            {isRegistering ? 'Zaten hesabınız var mı?' : 'Hesabınız yok mu?'}
                        </Typography>
                        <Button
                            variant="outlined"
                            color="inherit"
                            onClick={() => setIsRegistering(!isRegistering)}
                        >
                            {isRegistering ? 'Giriş Yap' : 'Kayıt Ol'}
                        </Button>
                    </Grid>

                    {/* Sağ panel (Form) */}
                    <Grid item xs={12} md={7} p={4}>
                        {error && (
                            <Alert severity="error" sx={{mb: 2}}>
                                {error}
                            </Alert>
                        )}

                        {isRegistering ? (
                            <form onSubmit={handleRegisterSubmit}>
                                <Typography variant="h6" textAlign="center" mb={3}>
                                    Kayıt Ol
                                </Typography>
                                <TextField
                                    fullWidth
                                    label="Ad"
                                    margin="normal"
                                    value={registerFirstName}
                                    onChange={(e) => setRegisterFirstName(e.target.value)}
                                    required
                                />
                                <TextField
                                    fullWidth
                                    label="Soyad"
                                    margin="normal"
                                    value={registerLastName}
                                    onChange={(e) => setRegisterLastName(e.target.value)}
                                    required
                                />
                                <TextField
                                    fullWidth
                                    label="Email"
                                    type="email"
                                    margin="normal"
                                    value={registerEmail}
                                    onChange={(e) => setRegisterEmail(e.target.value)}
                                    required
                                />
                                <TextField
                                    fullWidth
                                    label="Şifre"
                                    type="password"
                                    margin="normal"
                                    value={registerPassword}
                                    onChange={(e) => setRegisterPassword(e.target.value)}
                                    required
                                />
                                <Button type="submit" variant="contained" fullWidth sx={{mt: 2}}>
                                    Kayıt Ol
                                </Button>
                            </form>
                        ) : (
                            <form onSubmit={handleLoginSubmit}>
                                <Typography variant="h6" textAlign="center" mb={3}>
                                    Giriş Yap
                                </Typography>
                                <TextField
                                    fullWidth
                                    label="Email"
                                    type="email"
                                    margin="normal"
                                    value={loginEmail}
                                    onChange={(e) => setLoginEmail(e.target.value)}
                                    required
                                />
                                <TextField
                                    fullWidth
                                    label="Şifre"
                                    type="password"
                                    margin="normal"
                                    value={loginPassword}
                                    onChange={(e) => setLoginPassword(e.target.value)}
                                    required
                                />
                                <Button type="submit" variant="contained" fullWidth sx={{mt: 2}}>
                                    Giriş Yap
                                </Button>
                            </form>
                        )}
                    </Grid>
                </Grid>
            </Card>
        </Box>
    );
};

export default AuthForm;
