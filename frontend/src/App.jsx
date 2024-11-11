import {   Route, Routes } from 'react-router-dom'
import Navbar from './components/Navbar'
import Footer from './components/Footer'
import Home from './Pages/Home'
import Doctors from './Pages/Doctors'
import About from './Pages/About'
import Contact from './Pages/Contact'
import Login from './Pages/Login'
import Appointment from './Pages/Appointment'
import { ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import MyProfile from './Pages/MyProfile'
import MyAppointments from './Pages/MyAppointments'



function App() {
  return (
    <div className='mx-4 sm:mx-[10%]'>
 {/* Place the ToastContainer here without wrapping other components */}
      <ToastContainer />
       <Navbar />
      
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/doctors" element={<Doctors />} />
        <Route path='/doctors/:speciality' element={<Doctors />} />
        <Route path='/appointment/:docId' element={<Appointment />} />
        <Route path='/my-appointments' element={<MyAppointments />} />
        <Route path='/my-profile' element={<MyProfile />} />
        <Route path='/about' element={<About />} />
        <Route path='/contact' element={<Contact />} />
        <Route path='/login' element={<Login />} />      
      </Routes>
      <Footer />
     
    </div>
  )
}

export default App
