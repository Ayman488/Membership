const allUniversitiesData = {
    "Sakarya University": {
        "برامج البكالوريوس": {
            "Bilgisayar ve Bilişim Bilimleri Fakültesi": ["Bilgisayar Mühendisliği", "Bilişim Sistemleri Mühendisliği", "Bilişim Sistemleri ve Teknolojileri", "Siber Güvenlik Mühendisliği", "Veri Bilimi ve Analitiği", "Yazılım Mühendisliği (İngilizce)"],
            "Diş Hekimliği Fakültesi": ["Diş Hekimliği"],
            "Eğitim Fakültesi (Hendek)": ["Fen Bilgisi Öğretmenliği", "İlköğretim Matematik Öğretmenliği", "İngilizce Öğretmenliği (İngilizce)", "Okul Öncesi Öğretmenliği", "Özel Eğitim Öğretmenliği", "Rehberlik ve Psikolojik Danışmanlık", "Sınıf Öğretmenliği", "Sosyal Bilgiler Öğretmenliği", "Türkçe Öğretmenliği"],
            "Fen Fakültesi": ["Biyoloji", "Fizik", "Kimya", "Matematik"],
            "Hukuk Fakültesi": ["Hukuk"],
            "İlahiyat Fakültesi": ["İlahiyat", "İlahiyat (M.T.O.K.)"],
            "İletişim Fakültesi": ["Gazetecilik", "Halkla İlişkiler ve Reklamcılık", "Radyo, Televizyon ve Sinema", "Yeni Medya ve İletişim"],
            "İnsan ve Toplum Bilimleri Fakültesi": ["Alman Dili ve Edebiyatı", "Almanca Mütercim ve Tercümanlık", "Coğrafya", "Felsefe", "İngilizce Mütercim ve Tercümanlık", "Psikoloji", "Sanat Tarihi", "Sosyal Hizmet", "Sosyoloji", "Tarih", "Türk Dili ve Edebiyati"],
            "İşletme Fakültesi": ["İnsan Kaynakları Yönetimi", "İşletme", "İşletme (İngilizce)", "Sağlık Yönetimi", "Uluslararası Ticaret ve Lojistik", "Yönetim Bilişim Sistemleri"],
            "Mühendislik Fakültesi": ["Çevre Mühendisliği", "Elektrik-Elektronik Mühendisliği", "Endüstri Mühendisliği", "Gıda Mühendisliği", "İnşaat Mühendisliği", "Makine Mühendisliği", "Metalurji ve Malzeme Mühendisliği", "Metalurji ve Malzeme Mühendisliği (İngilizce)"],
            "Sağlık Bilimleri Fakültesi": ["Ebelik", "Hemşirelik"],
            "Sanat, Tasarım ve Mimarlık Fakültesi": ["Görsel İletişim Tasarımı", "Mimarlık"],
            "Siyasal Bilgiler Fakültesi": ["Çalışma Ekonomisi ve Endüstri İlişkileri", "Ekonometri", "İktisat", "İslam İktisadı ve Finans", "Maliye", "Siyaset Bilimi ve Kamu Yönetimi", "Uluslararası İlişkiler"],
            "Tıp Fakültesi": ["Tıp"]
        },
        "برامج الدبلوم (Meslek Yüksekokulu)": {
            "Adapazarı Meslek Yüksekokulu": ["Acil Durum ve Afet Yönetimi", "Bilgisayar Programcılığı", "İnternet ve Ağ Teknolojileri", "Sağlık Bilgi Sistemleri Teknikerliği", "Web Tasarımı ve Kodlama"],
            "Sakarya Sağlık Hizmetleri MYO": ["Ağız ve Diş Sağlığı", "Anestezi", "Çocuk Gelişimi", "Fizyoterapi", "İlk ve Acil Yardım", "Optisyenlik", "Tıbbi Dokümantasyon ve Sekreterlik", "Tıbbi Laboratuvar Teknikleri", "Yaşlı Bakımı"]
        }
    },
    "Sakarya Uygulamalı Bilimler Üniversitesi": {
        "برامج البكالوريوس": {
            "Teknoloji Fakültesi": ["Bilgisayar Mühendisliği", "Bilgisayar Mühendisliği (M.T.O.K.)", "Elektrik-Elektronik Mühendisliği", "İnşaat Mühendisliği", "Makine Mühendisliği", "Makine Mühendisliği (M.T.O.K.)", "Mekatronik Mühendisliği", "Mekatronik Mühendisliği (M.T.O.K.)", "Metalurji ve Malzeme Mühendisliği"],
            "Ziraat Fakültesi": ["Bahçe Bitkileri", "Bitki Koruma", "Peyzaj Mimarlığı", "Tarla Bitkileri"],
            "Sağlık Bilimleri Fakültesi (Akyazı)": ["Fizyoterapi ve Rehabilitasyon", "Hemşirelik", "Sağlık Yönetimi"],
            "Turizm Fakültesi (Sapanca)": ["Gastronomi ve Mutfak Sanatları", "Rekreasyon Yönetimi", "Turizm İşletmeciliği", "Turizm Rehberliği"],
            "Uygulamalı Bilimler Fakültesi": ["Finans ve Bankacılık", "Uluslararası Ticaret ve Finansman", "Uluslararası Ticaret ve Lojistik"],
            "Spor Bilimleri Fakültesi": ["Spor Yöneticiliği"]
        }
    }
};

document.addEventListener('DOMContentLoaded', function () {

    const uniSelect = document.getElementById('universitySelect');
    const facSelect = document.getElementById('facultySelect');
    const deptSelect = document.getElementById('departmentSelect');

    // عند تغيير الجامعة
    uniSelect.addEventListener('change', function () {
        const selectedUni = this.value;
        facSelect.innerHTML = '<option value="">اختر الكلية</option>';
        deptSelect.innerHTML = '<option value="">اختر الكلية أولاً</option>';
        deptSelect.disabled = true;

        if (selectedUni && allUniversitiesData[selectedUni]) {
            facSelect.disabled = false;
            const categories = allUniversitiesData[selectedUni];
            for (const cat in categories) {
                const group = document.createElement('optgroup');
                group.label = cat;
                for (const fac in categories[cat]) {
                    const opt = document.createElement('option');
                    opt.value = fac;
                    opt.textContent = fac;
                    group.appendChild(opt);
                }
                facSelect.appendChild(group);
            }
        } else {
            facSelect.disabled = true;
        }
    });

    facSelect.addEventListener('change', function () {
        const selectedUni = uniSelect.value;
        const selectedFac = this.value;
        deptSelect.innerHTML = '<option value="">اختر التخصص</option>';

        if (selectedUni && selectedFac) {
            deptSelect.disabled = false;
            let depts = [];
            const categories = allUniversitiesData[selectedUni];
            for (const cat in categories) {
                if (categories[cat][selectedFac]) {
                    depts = categories[cat][selectedFac];
                    break;
                }
            }
            depts.forEach(function (d) {
                const opt = document.createElement('option');
                opt.value = d;
                opt.textContent = d;
                deptSelect.appendChild(opt);
            });
        } else {
            deptSelect.disabled = true;
        }
    });

    setInterval(function () {
        fetch(window.location.href);
    }, 300000);

});