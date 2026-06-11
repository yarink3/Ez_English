/**
 * Renders a lesson-item's "image" field either as an emoji glyph (when the
 * value is not a URL/path) or as an <img>. Lets the seed data use compact
 * emoji ("🐱", "5") instead of forcing us to ship an SVG for every concept.
 */
export default function ItemImage({
  image,
  alt = '',
  className,
  emojiSize = '4.5rem',
}: {
  image: string
  alt?: string
  className?: string
  emojiSize?: string
}) {
  const isUrl =
    image.startsWith('/') ||
    image.startsWith('http://') ||
    image.startsWith('https://') ||
    image.startsWith('data:')

  if (isUrl) {
    return <img src={image} alt={alt} className={className} draggable={false} />
  }
  return (
    <span
      role="img"
      aria-label={alt || image}
      className={className}
      style={{ fontSize: emojiSize, lineHeight: 1, userSelect: 'none' }}
    >
      {image}
    </span>
  )
}
