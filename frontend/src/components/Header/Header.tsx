import { FC } from 'react';
import { HeaderWrapper, Brand, AppTitle } from './Header.styled';
import { Link } from 'react-router-dom';

interface HeaderProps { }

const Header: FC<HeaderProps> = () => (
   <HeaderWrapper>
      <nav className="navbar navbar-expand-lg navbar-light bg-light">
         <div className="container">
            <Brand className="navbar-brand fs-1" href="https://cmclellen.github.io/">Craig McLellen </Brand><AppTitle className="fs-4">Reservation System</AppTitle>
            <button className="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav" aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation">
               <span className="navbar-toggler-icon"></span>
            </button>
            <div className="collapse navbar-collapse justify-content-end" id="navbarNav">
               <ul className="navbar-nav">
                  <li className="nav-item">
                     <Link className="nav-link active" aria-current="page" to="/design">Design</Link>
                  </li>
                  <li className="nav-item">
                     <Link className="nav-link active" aria-current="page" to="/demo">Demo</Link>
                  </li>
               </ul>
            </div>
         </div>
      </nav>
   </HeaderWrapper>
);

export default Header;
