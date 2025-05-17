import Cookies from 'js-cookie';
import { useContext, useState } from 'react';
import { useForm } from 'react-hook-form';
import { useNavigate, useLocation } from 'react-router-dom';
import { AdminContext } from '../contexts/AdminContext.jsx';
import AdminServices from '../services/AdminServices.js';
import { notifyError, notifySuccess } from '../utils/toast.jsx';



const useLoginSubmit = () => {
  const navigate = useNavigate(); // useHistory yerine useNavigate kullanılıyor
  const location = useLocation(); // Yönlendirme sonrası için mevcut konumu almak üzere
  const { dispatch } = useContext(AdminContext);
  const [loading, setLoading] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm();

  const onSubmit = ({ name, email, verifyEmail, password, role }) => {
    setLoading(true);

    if (verifyEmail) {
      AdminServices.forgetPassword({ verifyEmail })
        .then((res) => {
          setLoading(false);
          notifySuccess(res.message);
        })
          .catch((err) => {
            const message =
                err?.response?.data?.message || err?.message || 'Bilinmeyen bir hata oluştu';
            notifyError(message);
            setLoading(false);
          });
    } else if (name) {
      AdminServices.registerAdmin({ name, email, password, role })
        .then((res) => {
          if (res) {
            console.log(res);
            setLoading(false);
            notifySuccess('Register Success!');
            dispatch({ type: 'USER_LOGIN', payload: res });
            Cookies.set('adminInfo', JSON.stringify(res));
            history.replace('/');
          }
        })
          .catch((err) => {
            const message =
                err?.response?.data?.message || err?.message || 'Bilinmeyen bir hata oluştu';
            notifyError(message);
            setLoading(false);
          });
    } else {
      AdminServices.loginAdmin({ email, password })
          .then((res) => {
            if (res) {
              setLoading(false);
              notifySuccess('Login Success!');
              dispatch({ type: 'USER_LOGIN', payload: res });
              Cookies.set('adminInfo', JSON.stringify(res));
              navigate('/', { replace: true }); // ✅ doğru kullanım
            }
          })
          .catch((err) => {
            const message =
                err?.response?.data?.message || err?.message || 'Bilinmeyen bir hata oluştu';
            notifyError(message);
            setLoading(false);
          });
    }
  };
  return {
    onSubmit,
    register,
    handleSubmit,
    errors,
    loading,
  };
};

export default useLoginSubmit;
