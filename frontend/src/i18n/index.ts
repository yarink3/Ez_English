import i18n from 'i18next'
import { initReactI18next } from 'react-i18next'
import LanguageDetector from 'i18next-browser-languagedetector'

import he from './locales/he/common.json'
import en from './locales/en/common.json'

void i18n
  .use(LanguageDetector)
  .use(initReactI18next)
  .init({
    resources: {
      he: { common: he },
      en: { common: en },
    },
    fallbackLng: 'en',
    supportedLngs: ['he', 'en'],
    defaultNS: 'common',
    interpolation: { escapeValue: false },
    detection: {
      order: ['localStorage', 'navigator', 'htmlTag'],
      caches: ['localStorage'],
    },
  })

const applyDir = (lng: string) => {
  const dir = lng.startsWith('he') ? 'rtl' : 'ltr'
  document.documentElement.setAttribute('dir', dir)
  document.documentElement.setAttribute('lang', lng)
}

applyDir(i18n.language || 'he')
i18n.on('languageChanged', applyDir)

export default i18n
