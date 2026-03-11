import { FaGithub, FaLinkedin, FaWhatsapp } from "react-icons/fa";
import { FiGlobe } from "react-icons/fi";

const Footer = () => {
  const currentYear = new Date().getFullYear();

  return (
    <footer className="bg-white border-t py-4 mt-auto">
      <div className="max-w-[2520px] mx-auto xl:px-20 md:px-10 sm:px-2 px-4">
        <div className="flex flex-col md:flex-row items-center justify-between gap-4 text-[14px] text-neutral-800">
          {/* الجانب الأيسر: الحقوق والروابط الأساسية */}
          <div className="flex flex-col md:flex-row items-center gap-2 order-2 md:order-1">
            <div className="font-light">© {currentYear} Airbnb Clone, Inc.</div>
            <span className="hidden md:inline">·</span>
            <div className="hover:underline cursor-pointer font-light">
              Privacy
            </div>
            <span className="hidden md:inline">·</span>
            <div className="hover:underline cursor-pointer font-light">
              Terms
            </div>
            <span className="hidden md:inline">·</span>
            <div className="hover:underline cursor-pointer font-light">
              Sitemap
            </div>
          </div>

          {/* الجانب الأيمن: بياناتك الشخصية (التواصل) */}
          <div className="flex flex-row items-center gap-6 order-1 md:order-2">
            {/* اللغة والعملة (ستايل Airbnb) */}
            <div className="hidden sm:flex flex-row items-center gap-2 hover:underline cursor-pointer font-semibold">
              <FiGlobe size={18} />
              <span>English (US)</span>
            </div>
            <div className="hidden sm:flex flex-row items-center gap-2 hover:underline cursor-pointer font-semibold">
              <span>$ USD</span>
            </div>

            {/* روابط التواصل الاجتماعي الخاصة بك */}
            <div className="flex flex-row items-center gap-4 border-l pl-6 border-neutral-300">
              {/* GitHub */}
              <a
                href="https://github.com/DeveloperZIAD"
                target="_blank"
                rel="noopener noreferrer"
                className="hover:text-black transition"
                title="GitHub"
              >
                <FaGithub size={20} />
              </a>

              {/* LinkedIn */}
              <a
                href="https://www.linkedin.com/in/dev-zeyad-fullstack/"
                target="_blank"
                rel="noopener noreferrer"
                className="hover:text-[#0077b5] transition"
                title="LinkedIn"
              >
                <FaLinkedin size={20} />
              </a>

              {/* WhatsApp / Phone */}
              <a
                href="https://wa.me/+201012613765"
                target="_blank"
                rel="noopener noreferrer"
                className="hover:text-[#25D366] transition"
                title="Contact Me"
              >
                <FaWhatsapp size={20} />
              </a>
            </div>
          </div>
        </div>
      </div>
    </footer>
  );
};

export default Footer;
